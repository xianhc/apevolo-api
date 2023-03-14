using System;
using ApeVolo.Common.Global;
using ApeVolo.EventBus.EventBusRabbitMQ;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace ApeVolo.Api.Extensions;

/// <summary>
/// rabbitmq启动器
/// </summary>
public static class RabbitMQSetup
{
    public static void AddRabbitMQSetup(this IServiceCollection services)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));

        if (AppSettings.GetValue<bool>("RabbitMQ", "Enabled"))
        {
            services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
            {
                var factory = new ConnectionFactory
                {
                    HostName = AppSettings.GetValue("RabbitMQ", "Connection"),
                    UserName = AppSettings.GetValue("RabbitMQ", "Username"),
                    Password = AppSettings.GetValue("RabbitMQ", "Password"),
                    DispatchConsumersAsync = true
                };
                var retryCount = AppSettings.GetValue<int>("RabbitMQ", "RetryCount");
                return new RabbitMQPersistentConnection(factory, retryCount);
            });
        }
    }
}