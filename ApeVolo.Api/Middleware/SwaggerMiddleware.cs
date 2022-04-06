using System;
using System.IO;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Global;
using JetBrains.Annotations;
using log4net;
using Microsoft.AspNetCore.Builder;

namespace ApeVolo.Api.Middleware;

/// <summary>
/// Swagger UI 中间件
/// </summary>
public static class SwaggerMiddleware
{
    private static readonly ILog Log = LogManager.GetLogger(typeof(SwaggerMiddleware));

    public static void UseSwaggerMiddleware(this IApplicationBuilder app, [CanBeNull] Func<Stream> streamHtml)
    {
        if (app.IsNull())
            throw new ArgumentNullException(nameof(app));

        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint($"/swagger/{AppSettings.GetValue("Swagger", "Name")}/swagger.json",
                AppSettings.GetValue("Swagger", "Version"));

            var stream = streamHtml?.Invoke();
            if (stream == null)
            {
                const string msg = "index.html属性错误";
                Log.Error(msg);
                throw new Exception(msg);
            }

            c.IndexStream = () => stream;
            c.RoutePrefix = string.Empty;
        });
    }
}