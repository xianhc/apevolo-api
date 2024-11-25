using System;
using Ape.Volo.Common;
using Ape.Volo.Common.Caches;
using Ape.Volo.Common.Caches.Distributed;
using Ape.Volo.Common.Caches.Redis;
using Ape.Volo.Common.ConfigOptions;
using Ape.Volo.Common.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Ape.Volo.Api.Extensions;

/// <summary>
/// 缓存启动器
/// </summary>
public static class CacheSetup
{
    public static void AddCacheSetup(this IServiceCollection services)
    {
        if (services.IsNull())
            throw new ArgumentNullException(nameof(services));
        services.AddDistributedMemoryCache(); //session需要

        if (App.GetOptions<SystemOptions>().UseRedisCache)
        {
            services.AddSingleton<ICache, RedisCache>();
            return;
        }

        services.AddSingleton<ICache, DistributedCache>();
    }
}
