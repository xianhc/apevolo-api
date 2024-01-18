using System;
using Ape.Volo.Common.ConfigOptions;
using Ape.Volo.EventBus.EventBusRabbitMQ;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace Ape.Volo.Api.Extensions;

/// <summary>
/// rabbitmq启动器
/// </summary>
public static class RabbitMqSetup
{
    public static void AddRabbitMqSetup(this IServiceCollection services, Configs configs)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));

        if (configs.Middleware.RabbitMq.Enabled)
        {
            services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
            {
                var factory = new ConnectionFactory
                {
                    HostName = configs.Rabbit.Connection,
                    UserName = configs.Rabbit.Username,
                    Password = configs.Rabbit.Password,
                    DispatchConsumersAsync = true
                };
                var retryCount = configs.Rabbit.RetryCount;
                return new RabbitMQPersistentConnection(factory, retryCount);
            });
        }
    }
}
