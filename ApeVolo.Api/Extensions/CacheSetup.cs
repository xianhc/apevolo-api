using System;
using ApeVolo.Common.Caches;
using ApeVolo.Common.Caches.Distributed;
using ApeVolo.Common.Caches.Redis;
using ApeVolo.Common.ConfigOptions;
using ApeVolo.Common.Extention;
using Microsoft.Extensions.DependencyInjection;

namespace ApeVolo.Api.Extensions;

/// <summary>
/// 缓存启动器
/// </summary>
public static class CacheSetup
{
    public static void AddCacheSetup(this IServiceCollection services, Configs configs)
    {
        if (services.IsNull())
            throw new ArgumentNullException(nameof(services));
        //也可以增加MemoryCache选项，但是MemoryCache不支持异步操作，需要自行实现
        if (configs.CacheOption.RedisCacheSwitch.Enabled)
        {
            //开启了redis就优先使用redis
            services.AddSingleton<ICache, RedisCache>();
        }
        else if (configs.CacheOption.DistributedCacheSwitch.Enabled)
        {
            services.AddDistributedMemoryCache();
            services.AddSingleton<ICache, DistributedCache>();
        }
        else
        {
            //都没有默认使用DistributedCache 防止异常
            services.AddDistributedMemoryCache();
            services.AddSingleton<ICache, DistributedCache>();
        }
    }
}
