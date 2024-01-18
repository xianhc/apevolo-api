using System;
using Ape.Volo.Common.Caches.Redis.Attributes;
using Ape.Volo.Common.Caches.Redis.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Ape.Volo.Common.Caches.Redis.MessageQueue;

/// <summary>
/// redis消息队列中间件
/// https://github.com/wmowm/InitQ
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds and configures the consistence services for the consistency.
    /// </summary>
    /// <param name="services">The services available in the application.</param>
    /// <param name="setupAction">An action to configure the <see cref="RedisQueueOptions" />.</param>
    /// <returns>An <see cref="InitBuilder" /> for application services.</returns>
    public static InitBuilder AddRedisMq(this IServiceCollection services, Action<RedisQueueOptions> setupAction)
    {
        if (setupAction == null)
        {
            throw new ArgumentNullException(nameof(setupAction));
        }

        var options = new RedisQueueOptions();
        setupAction(options);


        services.Configure(setupAction);


        services.AddHostedService<HostedService>();


        if (options.ListSubscribe != null)
        {
            foreach (var item in options.ListSubscribe)
            {
                services.TryAddSingleton(item);
            }

            services.AddSingleton(serviceProvider =>
            {
                Func<Type, IRedisSubscribe> accesor = key =>
                {
                    foreach (var item in options.ListSubscribe)
                    {
                        if (key == item)
                        {
                            return serviceProvider.GetService(item) as IRedisSubscribe;
                        }
                    }

                    throw new ArgumentException($"不支持的DI Key: {key}");
                };
                return accesor;
            });
        }

        return new InitBuilder(services);
    }
}
