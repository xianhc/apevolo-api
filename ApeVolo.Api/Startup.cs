using System.Reflection;
using ApeVolo.Api.Extensions;
using ApeVolo.Api.Filter;
using ApeVolo.Api.Middleware;
using ApeVolo.Common.ClassLibrary;
using ApeVolo.Common.DI;
using ApeVolo.Common.Global;
using ApeVolo.Common.SnowflakeIdHelper;
using ApeVolo.Common.WebApp;
using ApeVolo.Entity.Seed;
using ApeVolo.IBusiness.Interface.System.Task;
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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

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
        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddSingleton(Configuration);
        services.AddLogging();
        services.Configure<KestrelServerOptions>(options => { options.AllowSynchronousIO = true; });
        services.Configure<IISServerOptions>(options => { options.AllowSynchronousIO = true; });
        services.AddMemoryCacheSetup();
        services.AddRedisCacheSetup();
        services.AddSqlsugarSetup();
        services.AddDbSetup();
        services.AddAutoMapperSetup();
        services.AddCorsSetup();
        services.AddMiniProfilerSetup();
        services.AddSwaggerSetup();
        services.AddQuartzNetJobSetup();
        services.AddAuthorizationSetup();
        services.AddSignalR().AddNewtonsoftJsonProtocol();
        services.AddBrowserDetection();
        services.AddRedisInitMqSetup();
        services.AddIpStrategyRateLimitSetup(Configuration);
        services.AddRabbitMQSetup();
        services.AddEventBusSetup();
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
    }

    public void ConfigureContainer(ContainerBuilder builder)
    {
        builder.RegisterModule(new AutofacRegister());
    }

    public void Configure(IApplicationBuilder app, MyContext myContext,
        IQuartzNetService quartzNetService,
        ISchedulerCenterService schedulerCenter, ILoggerFactory loggerFactory)
    {
        var locOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
        if (locOptions != null) app.UseRequestLocalization(locOptions.Value);
        //获取远程真实ip,如果不是nginx代理部署可以不要
        app.UseMiddleware<RealIpMiddleware>();
        //IP限流
        app.UseIpLimitMiddleware();
        //日志
        loggerFactory.AddLog4Net();
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
        app.UseCors(AppSettings.GetValue("Cors", "PolicyName"));
        //静态文件
        app.UseStaticFiles();
        //cookie
        app.UseCookiePolicy();
        //错误页
        app.UseStatusCodePages();
        app.UseRouting();

        app.UseCors("IpPolicy");
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

        app.UseDataSeederMiddleware(myContext);
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