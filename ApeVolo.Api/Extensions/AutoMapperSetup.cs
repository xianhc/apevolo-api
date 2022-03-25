using System;
using ApeVolo.Api.AutoMapper;
using ApeVolo.Common.Extention;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace ApeVolo.Api.Extensions;

/// <summary>
/// 实体映射启动器
/// </summary>
public static class AutoMapperSetup
{
    public static void AddAutoMapperSetup(this IServiceCollection services)
    {
        if (services.IsNull()) throw new ArgumentNullException(nameof(services));

        services.AddAutoMapper(typeof(AutoMapperConfig));
        AutoMapperConfig.RegisterMappings();
    }
}