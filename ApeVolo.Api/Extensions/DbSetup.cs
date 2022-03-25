using System;
using ApeVolo.Common.Extention;
using ApeVolo.Entity.Seed;
using Microsoft.Extensions.DependencyInjection;

namespace ApeVolo.Api.Extensions;

/// <summary>
/// 数据库上下文启动器
/// </summary>
public static class DbSetup
{
    public static void AddDbSetup(this IServiceCollection services)
    {
        if (services.IsNull()) throw new ArgumentNullException(nameof(services));

        services.AddScoped<SeedData>();
        services.AddScoped<MyContext>();
    }
}