using ApeVolo.Api.AutoMapper;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using System;
using ApeVolo.Common.Extention;

namespace ApeVolo.Api.Extensions
{
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
}