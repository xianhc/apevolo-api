using System;
using Ape.Volo.Api.MQ.Rabbit.EventHandling;
using Ape.Volo.Api.MQ.Rabbit.Events;
using Ape.Volo.Common.ConfigOptions;
using Ape.Volo.EventBus;
using Ape.Volo.EventBus.Abstractions;
using Ape.Volo.EventBus.EventBusRabbitMQ;
using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Ape.Volo.Api.Extensions;

/// <summary>
/// 事件总线启动器
/// </summary>
public static class EventBusSetup
{
    public static void AddEventBusSetup(this IServiceCollection services, Configs configs)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));

        if (configs.EventBus.Enabled)
        {
            var subscriptionClientName = configs.EventBus.SubscriptionClientName;

            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();
            services.AddTransient<UserQueryIntegrationEventHandler>();

            if (configs.Middleware.RabbitMq.Enabled)
            {
                services.AddSingleton<IEventBus, EventBusRabbitMQ>(sp =>
                {
                    var rabbitMqPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
                    var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
                    var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();
                    var retryCount = configs.Rabbit.RetryCount;

                    return new EventBusRabbitMQ(sp, rabbitMqPersistentConnection, iLifetimeScope,
                        subscriptionClientName, eventBusSubcriptionsManager, retryCount);
                });
            }
        }
    }


    public static void ConfigureEventBus(this IApplicationBuilder app)
    {
        var configs = app.ApplicationServices.GetRequiredService<IOptionsMonitor<Configs>>().CurrentValue;
        if (configs.EventBus.Enabled)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();

            eventBus.Subscribe<UserQueryIntegrationEvent, UserQueryIntegrationEventHandler>();
        }
    }
}
