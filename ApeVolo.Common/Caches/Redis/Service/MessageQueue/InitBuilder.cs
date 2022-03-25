using System;
using Microsoft.Extensions.DependencyInjection;

namespace ApeVolo.Common.Caches.Redis.Service.MessageQueue;

public class InitBuilder
{
    public InitBuilder(IServiceCollection services)
    {
        Services = services;
    }

    /// <summary>
    /// Gets the <see cref="IServiceCollection" /> where MVC services are configured.
    /// </summary>
    public IServiceCollection Services { get; }


    /// <summary>
    /// Adds a scoped service of the type specified in serviceType with an implementation
    /// </summary>
    private InitBuilder AddScoped(Type serviceType, Type concreteType)
    {
        Services.AddScoped(serviceType, concreteType);
        return this;
    }

    /// <summary>
    /// Adds a singleton service of the type specified in serviceType with an implementation
    /// </summary>
    private InitBuilder AddSingleton(Type serviceType, Type concreteType)
    {
        Services.AddSingleton(serviceType, concreteType);
        return this;
    }
}