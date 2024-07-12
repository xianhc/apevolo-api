using System;
using Ape.Volo.Common;
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
    public static void AddRabbitMqSetup(this IServiceCollection services)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));

        if (App.GetOptions<MiddlewareOptions>().RabbitMq.Enabled)
        {
            var options = App.GetOptions<RabbitOptions>();
            services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
            {
                var factory = new ConnectionFactory
                {
                    HostName = options.Connection,
                    UserName = options.Username,
                    Password = options.Password,
                    DispatchConsumersAsync = true
                };
                var retryCount = options.RetryCount;
                return new RabbitMQPersistentConnection(factory, retryCount);
            });
        }
    }
}
