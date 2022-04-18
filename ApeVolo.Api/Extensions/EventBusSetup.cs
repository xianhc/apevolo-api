using System;
using ApeVolo.Api.MQ.Rabbit.EventHandling;
using ApeVolo.Api.MQ.Rabbit.Events;
using ApeVolo.Common.Global;
using ApeVolo.EventBus;
using ApeVolo.EventBus.Abstractions;
using ApeVolo.EventBus.EventBusRabbitMQ;
using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace ApeVolo.Api.Extensions;

/// <summary>
/// 事件总线启动器
/// </summary>
public static class EventBusSetup
{
    public static void AddEventBusSetup(this IServiceCollection services)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));

        if (AppSettings.GetValue<bool>("EventBus", "Enabled"))
        {
            var subscriptionClientName = AppSettings.GetValue("EventBus", "SubscriptionClientName");

            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();
            services.AddTransient<UserQueryIntegrationEventHandler>();

            if (AppSettings.GetValue<bool>("RabbitMQ", "Enabled"))
            {
                services.AddSingleton<IEventBus, EventBusRabbitMQ>(sp =>
                {
                    var rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
                    var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
                    var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();
                    var retryCount = AppSettings.GetValue<int>("RabbitMQ", "RetryCount");

                    return new EventBusRabbitMQ(sp, rabbitMQPersistentConnection, iLifetimeScope,
                        subscriptionClientName, eventBusSubcriptionsManager, retryCount);
                });
            }
        }
    }


    public static void ConfigureEventBus(this IApplicationBuilder app)
    {
        if (AppSettings.GetValue<bool>("EventBus", "Enabled"))
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();

            eventBus.Subscribe<UserQueryIntegrationEvent, UserQueryIntegrationEventHandler>();
        }
    }
}