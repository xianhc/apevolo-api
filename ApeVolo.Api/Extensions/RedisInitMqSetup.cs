using System;
using System.Collections.Generic;
using ApeVolo.Api.MQ.Redis;
using ApeVolo.Common.Caches.Redis.Service.MessageQueue;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Global;
using Microsoft.Extensions.DependencyInjection;

namespace ApeVolo.Api.Extensions;

/// <summary>
/// redis消息队列
/// </summary>
public static class RedisInitMqSetup
{
    public static void AddRedisInitMqSetup(this IServiceCollection services)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));

        //启动redis消息队列 必须先启动redis缓存
        if (AppSettings.GetValue<bool>("Middleware", "RedisMq", "Enabled"))
        {
            services.AddRedisMq(m =>
            {
                //没消息时挂起时长(毫秒)
                m.SuspendTime = AppSettings.GetValue("RedisConfig", "SuspendTime").ToInt();
                //每次消费消息间隔时间(毫秒)
                m.IntervalTime = AppSettings.GetValue("RedisConfig", "IntervalTime").ToInt();
                //显示日志
                m.ShowLog = AppSettings.GetValue("RedisConfig", "ShowLog").ToBool();
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