using System;
using System.Linq;
using Ape.Volo.Common.ConfigOptions;
using Ape.Volo.Common.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Ape.Volo.Api.Extensions;

/// <summary>
/// 跨域启动器
/// </summary>
public static class CorsSetup
{
    public static void AddCorsSetup(this IServiceCollection services, Configs configs)
    {
        if (services.IsNull()) throw new ArgumentNullException(nameof(services));

        services.AddCors(c =>
        {
            if (configs.Cors.EnableAll)
            {
                //允许任意跨域请求
                c.AddPolicy(configs.Cors.Name,
                    policy =>
                    {
                        policy
                            .SetIsOriginAllowed(host => true)
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials();
                    });
            }
            else
            {
                c.AddPolicy(configs.Cors.Name,
                    policy =>
                    {
                        policy
                            .WithOrigins(configs.Cors.Policy.Select(x => x.Domain).ToArray())
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            }
        });
    }
}
