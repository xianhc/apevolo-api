using System;
using ApeVolo.Common.Caches.Redis.Attributes;
using ApeVolo.Common.Caches.Redis.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ApeVolo.Common.Caches.Redis.Service.MessageQueue
{
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
        /// <param name="setupAction">An action to configure the <see cref="CapOptions" />.</param>
        /// <returns>An <see cref="CapBuilder" /> for application services.</returns>
        public static InitBuilder AddRedisMq(this IServiceCollection services, Action<RedisOptions> setupAction)
        {
            if (setupAction == null)
            {
                throw new ArgumentNullException(nameof(setupAction));
            }

            var options = new RedisOptions();
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
}