using System;
using ApeVolo.Common.Caches.Redis.Models;
using ApeVolo.Common.Caches.Redis.Service;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Global;
using Microsoft.Extensions.DependencyInjection;

namespace ApeVolo.Api.Extensions;

/// <summary>
/// redis启动器
/// </summary>
public static class RedisCacheSetup
{
    public static void AddRedisCacheSetup(this IServiceCollection services)
    {
        if (services.IsNull())
            throw new ArgumentNullException(nameof(services));
        var redisOptions = new RedisOptions
        {
            RedisHost = AppSettings.GetValue("RedisConfig", "RedisHost"),

            Port = AppSettings.GetValue("RedisConfig", "Port"),

            RedisName = AppSettings.GetValue("RedisConfig", "RedisName"),

            RedisPass = AppSettings.GetValue("RedisConfig", "RedisPass"),

            RedisIndex = AppSettings.GetValue<int>("RedisConfig", "RedisIndex"),

            ConnectTimeout = AppSettings.GetValue<int>("RedisConfig", "SyncTimeout"),

            SyncTimeout = AppSettings.GetValue<int>("RedisConfig", "ConnectTimeout"),

            KeepAlive = AppSettings.GetValue<int>("RedisConfig", "KeepAlive"),

            ConnectRetry = AppSettings.GetValue<int>("RedisConfig", "ConnectRetry"),

            AbortOnConnectFail = AppSettings.GetValue<bool>("RedisConfig", "AbortOnConnectFail"),

            AllowAdmin = AppSettings.GetValue<bool>("RedisConfig", "AllowAdmin")
        };

        services.AddSingleton(typeof(IRedisCacheService), new RedisCacheService(redisOptions));
    }
}