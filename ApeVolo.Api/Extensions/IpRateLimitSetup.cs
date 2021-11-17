using System;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ApeVolo.Api.Extensions
{
    /// <summary>
    /// IP限流启动器
    /// </summary>
    public static class IpRateLimitSetup
    {
        public static void AddIpStrategyRateLimitSetup(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            // 將速限計數器資料儲存在 Memory 中
            //services.AddMemoryCache();

            // 從 appsettings.json 讀取 IpRateLimiting 設定 
            services.Configure<IpRateLimitOptions>(configuration.GetSection("IpRateLimiting"));

            // 從 appsettings.json 讀取 Ip Rule 設定
            services.Configure<IpRateLimitPolicies>(configuration.GetSection("IpRateLimitPolicies"));

            // 注入 counter and IP Rules 
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            
            //如要使用redis 打开注释
            // services.AddSingleton<IIpPolicyStore, DistributedCacheIpPolicyStore>();
            // services.AddSingleton<IRateLimitCounterStore, DistributedCacheRateLimitCounterStore>();
            // services.AddDistributedRedisCache(option =>
            // {
            //     option.Configuration = configuration["Redis:ConnectionString"];
            //     option.InstanceName = configuration["Redis:InstanceName"];
            // });

            // the clientId/clientIp resolvers use it.
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // Rate Limit configuration 設定
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        }
    }
}
