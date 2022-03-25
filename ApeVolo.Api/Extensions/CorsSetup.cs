using System;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Global;
using Microsoft.Extensions.DependencyInjection;

namespace ApeVolo.Api.Extensions;

/// <summary>
/// 跨域启动器
/// </summary>
public static class CorsSetup
{
    public static void AddCorsSetup(this IServiceCollection services)
    {
        if (services.IsNull()) throw new ArgumentNullException(nameof(services));

        if (services == null) throw new ArgumentNullException(nameof(services));

        services.AddCors(c =>
        {
            if (AppSettings.GetValue("Cors", "EnableAllIPs").ToBool())
            {
                //允许任意跨域请求
                c.AddPolicy(AppSettings.GetValue("Cors", "PolicyName"),
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
                c.AddPolicy(AppSettings.GetValue("Cors", "PolicyName"),
                    policy =>
                    {
                        policy
                            .WithOrigins(AppSettings.GetValue("Cors", "IPs").Split(','))
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            }
        });
    }
}