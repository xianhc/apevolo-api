using System.Reflection;
using Ape.Volo.Api.Extensions;
using Ape.Volo.Api.Filter;
using Ape.Volo.Api.Mapster;
using Ape.Volo.Api.Middleware;
using Ape.Volo.Common;
using Ape.Volo.Common.ClassLibrary;
using Ape.Volo.Common.ConfigOptions;
using Ape.Volo.Common.Global;
using Ape.Volo.Common.Internal;
using Ape.Volo.Common.SnowflakeIdHelper;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Mapster;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

new IdHelperBootstrapper().SetWorkderId(1).Boot();

// 配置容器
builder.Host
    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        hostingContext.Configuration.ConfigureApplication();
        config.Sources.Clear();
        config.AddJsonFile(builder.Environment.IsDevelopment() ? "appsettings.Development.json" : "appsettings.json",
                optional: true, reloadOnChange: false)
            .AddJsonFile("IpRateLimit.json", optional: true, reloadOnChange: false);
    }).UseSerilogMiddleware()
    .ConfigureContainer<ContainerBuilder>(builder => { builder.RegisterModule(new AutofacRegister()); });
builder.ConfigureApplication();

// 配置服务
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
builder.Services.AddSingleton<IRegister, MapsterCustomMapper>();
builder.Services.AddSingleton<IMapper, Mapper>();
builder.Services.AddSingleton(new AppSettings(builder.Configuration));
builder.Services.AddOptionRegister();
// builder.Services.Configure<Configs>(configuration);
// var configs = configuration.Get<Configs>();
builder.Services.AddSerilogSetup();
builder.Services.Configure<KestrelServerOptions>(options => { options.AllowSynchronousIO = true; });
builder.Services.Configure<IISServerOptions>(options => { options.AllowSynchronousIO = true; });
builder.Services.AddCacheSetup();
builder.Services.AddSqlSugarSetup();
builder.Services.AddDbSetup();
builder.Services.AddCorsSetup();
builder.Services.AddMiniProfilerSetup();
builder.Services.AddSwaggerSetup();
builder.Services.AddQuartzNetJobSetup();
builder.Services.AddAuthorizationSetup();
builder.Services.AddBrowserDetection();
builder.Services.AddRedisInitMqSetup();
builder.Services.AddIpStrategyRateLimitSetup();
builder.Services.AddRabbitMqSetup();
builder.Services.AddEventBusSetup();
//services.AddLocalization(options => options.ResourcesPath = "Resources");
//services.AddMultiLanguages(op => op.LocalizationType = typeof(Common.Language));
builder.Services.AddControllers(options =>
    {
        // 异常过滤器
        options.Filters.Add<GlobalExceptionFilter>();
        // 审计过滤器
        options.Filters.Add<AuditingFilter>();
    })
    //.AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
    //.AddDataAnnotationsLocalization(typeof(Common.Language))
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
builder.Services.AddIpSearcherSetup();

// 配置中间件
var app = builder.Build();
app.ConfigureApplication();

//实体映射配置
var mapper = app.Services.GetRequiredService<IRegister>();
TypeAdapterConfig.GlobalSettings.Apply(mapper);

//IP限流
app.UseIpLimitMiddleware();
//var locOptions = app.Services.GetService<IOptions<RequestLocalizationOptions>>();
//if (locOptions != null) app.UseRequestLocalization(locOptions.Value);
//获取远程真实ip,如果不是nginx代理部署可以不要
app.UseMiddleware<RealIpMiddleware>();
//处理访问不存在的接口
//app.UseMiddleware<NotFoundMiddleware>();
app.UseSerilogRequestLogging();
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.Use(next => context =>
{
    context.Request.EnableBuffering();
    return next(context);
});

//autofac
//AutofacHelper.Container = app.Services.GetAutofacRoot();
//Swagger UI
app.UseSwaggerMiddleware(() => Assembly.GetExecutingAssembly().GetManifestResourceStream("Ape.Volo.Api.index.html"));
// CORS跨域
app.UseCors(App.GetOptions<CorsOptions>().Name);
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

//app.UseHttpMethodOverride();

//种子数据
app.UseDataSeederMiddleware();

//作业调度
app.UseQuartzNetJobMiddleware();

//事件总线配置订阅
app.ConfigureEventBus();

// 运行
app.Run();
