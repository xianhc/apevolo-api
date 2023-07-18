using System;
using ApeVolo.Common.Caches.Redis.Models;
using ApeVolo.Common.Caches.Redis.Service;
using ApeVolo.Common.ConfigOptions;
using ApeVolo.Common.Extention;
using Microsoft.Extensions.DependencyInjection;

namespace ApeVolo.Api.Extensions;

/// <summary>
/// redis启动器
/// </summary>
public static class RedisCacheSetup
{
    public static void AddRedisCacheSetup(this IServiceCollection services, Configs configs)
    {
        if (services.IsNull())
            throw new ArgumentNullException(nameof(services));
        var redisConfigs = configs.Redis;
        var redisOptions = new RedisOptions
        {
            Name = redisConfigs.Name,
            Host = redisConfigs.Host,
            Port = redisConfigs.Port,
            Password = redisConfigs.Password,
            Index = redisConfigs.Index,
            ConnectTimeout = redisConfigs.ConnectTimeout,
            SyncTimeout = redisConfigs.SyncTimeout,
            KeepAlive = redisConfigs.KeepAlive,
            ConnectRetry = redisConfigs.ConnectRetry,
            AbortOnConnectFail = redisConfigs.AbortOnConnectFail,
            AllowAdmin = redisConfigs.AllowAdmin,
            ShowLog = redisConfigs.ShowLog
        };

        services.AddSingleton(typeof(IRedisCacheService), new RedisCacheService(redisOptions));
    }
}
