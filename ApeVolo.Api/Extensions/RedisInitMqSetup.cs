using System;
using System.Collections.Generic;
using ApeVolo.Api.MQ.Redis;
using ApeVolo.Common.Caches.Redis.MessageQueue;
using ApeVolo.Common.ConfigOptions;
using Microsoft.Extensions.DependencyInjection;

namespace ApeVolo.Api.Extensions;

/// <summary>
/// redis消息队列
/// </summary>
public static class RedisInitMqSetup
{
    public static void AddRedisInitMqSetup(this IServiceCollection services, Configs configs)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));

        //启动redis消息队列 必须先启动redis缓存
        if (configs.CacheOption.RedisCacheSwitch.Enabled && configs.Middleware.RedisMq.Enabled)
        {
            services.AddRedisMq(m =>
            {
                //没消息时挂起时长(毫秒)
                m.SuspendTime = configs.Redis.SuspendTime;
                //每次消费消息间隔时间(毫秒)
                m.IntervalTime = configs.Redis.IntervalTime;
                //显示日志
                m.ShowLog = configs.Redis.ShowLog;
                //订阅者类
                m.ListSubscribe = new List<Type>
                {
                    typeof(EmailRedisSubscribe)
                    //多个类继续往下加
                };
            });
        }
    }
}
