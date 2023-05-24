using System;
using System.IO;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Global;
using ApeVolo.Common.Helper;
using ApeVolo.Common.Helper.Serilog;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
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
        if (AppSettings.GetValue<bool>("Swagger", "Enabled"))
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/swagger/{AppSettings.GetValue("Swagger", "Name")}/swagger.json",
                    AppSettings.GetValue("Swagger", "Version"));

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