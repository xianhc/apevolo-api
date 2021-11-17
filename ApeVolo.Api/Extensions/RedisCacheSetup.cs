using System;
using ApeVolo.Common.Caches.Redis;
using ApeVolo.Common.Caches.Redis.Models;
using ApeVolo.Common.Caches.Redis.Service;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Global;
using Microsoft.Extensions.DependencyInjection;

namespace ApeVolo.Api.Extensions
{
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

                RedisIndex = AppSettings.GetValue("RedisConfig", "RedisIndex").ToInt(),

                ConnectTimeout = AppSettings.GetValue("RedisConfig", "SyncTimeout").ToInt(),

                SyncTimeout = AppSettings.GetValue("RedisConfig", "ConnectTimeout").ToInt(),

                KeepAlive = AppSettings.GetValue("RedisConfig", "KeepAlive").ToInt(),

                ConnectRetry = AppSettings.GetValue("RedisConfig", "ConnectRetry").ToInt(),

                AbortOnConnectFail = AppSettings.GetValue("RedisConfig", "AbortOnConnectFail").ToBool(),

                AllowAdmin = AppSettings.GetValue("RedisConfig", "AllowAdmin").ToBool()
            };

            services.AddSingleton(typeof(IRedisCacheService), new RedisCacheService(redisOptions));
        }
    }
}