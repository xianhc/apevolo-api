using ApeVolo.Api.Aop;
using ApeVolo.Api.Extensions;
using ApeVolo.Api.Filter;
using ApeVolo.Api.Middleware;
using ApeVolo.Common.DI;
using ApeVolo.Common.Helper;
using ApeVolo.Common.Helper.Excel;
using ApeVolo.Common.SnowflakeIdHelper;
using ApeVolo.Common.WebApp;
using ApeVolo.IBusiness.Interface.Tasks;
using ApeVolo.QuartzNetService.service;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Extras.DynamicProxy;
using log4net;
using log4net.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Global;
using ApeVolo.Entity.Seed;

namespace ApeVolo.Api
{
    public class Startup
    {
        private IConfiguration Configuration { get; }
        private IWebHostEnvironment Env { get; }
        public static ILoggerRepository Repository { get; set; }
        private static readonly ILog Log = LogManager.GetLogger(typeof(GlobalExceptionFilter));

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Env = env;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddSingleton(new LogHelper(Env.ContentRootPath));
            services.AddSingleton(new ExcelHelper(Env.ContentRootPath));
            services.AddScoped<ICurrentUser, CurrentUser>();
            services.AddSingleton(Configuration);
            services.AddLogging();
            services.Configure<KestrelServerOptions>(options => { options.AllowSynchronousIO = true; });
            services.Configure<IISServerOptions>(options => { options.AllowSynchronousIO = true; });
            services.AddMemoryCacheSetup();
            //services.AddRedisCacheService(_ => RedisHelper.GetRedisOptions());
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


            services.AddControllers(options =>
                {
                    // 异常过滤器
                    options.Filters.Add(typeof(GlobalExceptionFilter));
                    // 审计过滤器
                    options.Filters.Add<AuditingFilter>();
                })
                .AddControllersAsServices()
                .AddNewtonsoftJson(options =>
                    {
                        //全局忽略循环引用
                        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                        //options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                        options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                    }
                );
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            #region DI依赖注入

            //AOP
            var cacheType = new List<Type>();
            builder.RegisterType<TransactionAop>();
            cacheType.Add(typeof(TransactionAop));
            builder.RegisterType<RedisAop>();
            cacheType.Add(typeof(RedisAop));

            // 获取所有待注入服务类
            var baseTypeService = typeof(IDependencyService);
            var diTypes = GlobalData.FxAllTypes
                .Where(x => baseTypeService.IsAssignableFrom(x) && x != baseTypeService).ToArray();
            builder.RegisterTypes(diTypes)
                .AsImplementedInterfaces()
                .PropertiesAutowired()
                .InstancePerDependency()
                .EnableInterfaceInterceptors()
                .InterceptedBy(cacheType.ToArray());


            // 获取所有待注入仓储类
            var diTypesRepository = typeof(IDependencyRepository);
            var diTypes2 = GlobalData.FxAllTypes
                .Where(x => diTypesRepository.IsAssignableFrom(x) && x != diTypesRepository).ToArray();
            builder.RegisterTypes(diTypes2)
                .AsImplementedInterfaces()
                .InstancePerDependency();


            //注册
            builder.RegisterType<DisposableContainer>()
                .As<IDisposableContainer>()
                .InstancePerLifetimeScope();

            #endregion
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, MyContext myContext,
            IQuartzNetService quartzNetService,
            ISchedulerCenterService schedulerCenter, ILoggerFactory loggerFactory)
        {
            app.UseMiddleware<RealIpMiddleware>();
            //IP限流
            app.UseIpLimitMiddleware();
            //日志
            loggerFactory.AddLog4Net();
            if (env.IsDevelopment())
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

            app.UseSeedDataMildd(myContext, env.WebRootPath);
            //作业调度
            app.UseQuartzNetJobMiddleware(quartzNetService, schedulerCenter);

            //雪花ID器
            new IdHelperBootstrapper().SetWorkderId(1).Boot();


            //List<object> items = new List<object>();
            //foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            //{
            //    items.Add(assembly);
            //}
        }
    }
}