using System;
using System.IO;
using ApeVolo.Common.ConfigOptions;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Helper.Serilog;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;

namespace ApeVolo.Api.Middleware;

/// <summary>
/// Swagger UI 中间件
/// </summary>
public static class SwaggerMiddleware
{
    private static readonly ILogger Logger = SerilogManager.GetLogger(typeof(SwaggerMiddleware));

    public static void UseSwaggerMiddleware(this IApplicationBuilder app, [CanBeNull] Func<Stream> streamHtml)
    {
        if (app.IsNull())
            throw new ArgumentNullException(nameof(app));
        var configs = app.ApplicationServices.GetRequiredService<IOptionsMonitor<Configs>>().CurrentValue;
        if (configs.Swagger.Enabled)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/swagger/{configs.Swagger.Name}/swagger.json", configs.Swagger.Version);

                var stream = streamHtml?.Invoke();
                if (stream == null)
                {
                    const string msg = "index.html属性错误";
                    Logger.Error(msg);
                    throw new Exception(msg);
                }

                c.IndexStream = streamHtml;
                c.RoutePrefix = "";
            });
        }
    }
}
