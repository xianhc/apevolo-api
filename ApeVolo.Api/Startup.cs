using System.Reflection;
using ApeVolo.Api.Extensions;
using ApeVolo.Api.Filter;
using ApeVolo.Api.Middleware;
using ApeVolo.Common.Caches;
using ApeVolo.Common.Caches.Distributed;
using ApeVolo.Common.ClassLibrary;
using ApeVolo.Common.ConfigOptions;
using ApeVolo.Common.DI;
using ApeVolo.Common.Global;
using ApeVolo.Common.SnowflakeIdHelper;
using ApeVolo.Common.WebApp;
using ApeVolo.Entity.Seed;
using ApeVolo.IBusiness.Interface.System;
using ApeVolo.QuartzNetService.service;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;

namespace ApeVolo.Api;

public class Startup
{
    private IConfiguration Configuration { get; }
    private IWebHostEnvironment WebHostEnvironment { get; }

    public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
    {
        Configuration = configuration;
        WebHostEnvironment = webHostEnvironment;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
        services.AddSingleton(new AppSettings(WebHostEnvironment));
        var configs = Configuration.Get<Configs>();
        services.Configure<Configs>(Configuration);
        services.AddSerilogSetup();
        services.Configure<KestrelServerOptions>(options => { options.AllowSynchronousIO = true; });
        services.Configure<IISServerOptions>(options => { options.AllowSynchronousIO = true; });
        services.AddCacheSetup(configs);
        services.AddSqlSugarSetup(configs);
        services.AddDbSetup();
        services.AddAutoMapperSetup();
        services.AddCorsSetup(configs);
        services.AddMiniProfilerSetup();
        services.AddSwaggerSetup(configs);
        services.AddQuartzNetJobSetup();
        services.AddAuthorizationSetup(configs);
        services.AddBrowserDetection();
        services.AddRedisInitMqSetup(configs);
        services.AddIpStrategyRateLimitSetup(Configuration);
        services.AddRabbitMqSetup(configs);
        services.AddEventBusSetup(configs);
        services.AddLocalization(options => options.ResourcesPath = "Resources");
        services.AddMultiLanguages(op => op.LocalizationType = typeof(Common.Language));

        services.AddControllers(options =>
            {
                // 异常过滤器
                options.Filters.Add(typeof(GlobalExceptionFilter));
                // 审计过滤器
                options.Filters.Add<AuditingFilter>();
            })
            .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
            .AddDataAnnotationsLocalization(typeof(Common.Language))
            .AddControllersAsServices()
            .AddNewtonsoftJson(options =>
                {
                    //全局忽略循环引用
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    // options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                    options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                    options.SerializerSettings.ContractResolver = new CustomContractResolver();
                }
            );
        services.AddScoped<ApeContext>();
    }

    public void ConfigureContainer(ContainerBuilder builder)
    {
        builder.RegisterModule(new AutofacRegister(Configuration));
    }

    public void Configure(IApplicationBuilder app, DataContext dataContext, IQuartzNetService quartzNetService,
        ISchedulerCenterService schedulerCenter, IOptionsMonitor<Configs> configs, ApeContext apeContext)
    {
        var locOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
        if (locOptions != null) app.UseRequestLocalization(locOptions.Value);
        //获取远程真实ip,如果不是nginx代理部署可以不要
        app.UseMiddleware<RealIpMiddleware>();
        app.UseSerilogRequestLogging();
        //IP限流
        app.UseIpLimitMiddleware();
        if (WebHostEnvironment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.Use(next => context =>
        {
            context.Request.EnableBuffering();

            return next(context);
        });

        //autofac
        AutofacHelper.Container = app.ApplicationServices.GetAutofacRoot();
        //Swagger UI
        app.UseSwaggerMiddleware(() =>
            GetType().GetTypeInfo().Assembly.GetManifestResourceStream("ApeVolo.Api.index.html"));
        // CORS跨域
        app.UseCors(configs.CurrentValue.Cors.Name);
        //静态文件
        app.UseStaticFiles();
        //cookie
        app.UseCookiePolicy();
        //错误页
        app.UseStatusCodePages();
        app.UseRouting();
        // 认证
        app.UseAuthentication();
        // 授权
        app.UseAuthorization();
        //性能监控
        app.UseMiniProfilerMiddleware();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
        });
        app.UseHttpMethodOverride();

        app.UseDataSeederMiddleware(dataContext);
        //作业调度
        app.UseQuartzNetJobMiddleware(quartzNetService, schedulerCenter);

        //雪花ID器
        new IdHelperBootstrapper().SetWorkderId(1).Boot();
        //事件总线配置订阅
        app.ConfigureEventBus();

        //List<object> items = new List<object>();
        //foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        //{
        //    items.Add(assembly);
        //}
    }
}
