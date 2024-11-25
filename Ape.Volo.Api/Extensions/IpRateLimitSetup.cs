using System;
using Ape.Volo.Common;
using Ape.Volo.Common.ConfigOptions;
using AspNetCoreRateLimit;
using Microsoft.Extensions.DependencyInjection;

namespace Ape.Volo.Api.Extensions;

/// <summary>
/// IP限流启动器
/// </summary>
public static class IpRateLimitSetup
{
    public static void AddIpStrategyRateLimitSetup(this IServiceCollection services)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        services.Configure<IpRateLimitOptions>(App.Configuration.GetSection("IpRateLimiting"));
        services.Configure<IpRateLimitPolicies>(App.Configuration.GetSection("IpRateLimitPolicies"));

        if (App.GetOptions<SystemOptions>().UseRedisCache)
        {
            var redisOptions = App.GetOptions<RedisOptions>();
            services.AddStackExchangeRedisCache(option =>
            {
                option.Configuration = redisOptions.Host + ":" + redisOptions.Port;
                option.InstanceName = "rateLimit:";
            });
            services.AddSingleton<IIpPolicyStore, DistributedCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, DistributedCacheRateLimitCounterStore>();
        }
        else
        {
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
        }

        services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
        services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
    }
}
