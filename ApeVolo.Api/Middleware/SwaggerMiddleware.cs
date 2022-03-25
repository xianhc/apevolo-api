using System;
using System.IO;
using ApeVolo.Common.Extention;
using log4net;
using Microsoft.AspNetCore.Builder;

namespace ApeVolo.Api.Middleware;

/// <summary>
/// Swagger UI 中间件
/// </summary>
public static class SwaggerMiddleware
{
    private static readonly ILog Log = LogManager.GetLogger(typeof(SwaggerMiddleware));

    public static void UseSwaggerMiddleware(this IApplicationBuilder app, Func<Stream> streamHtml)
    {
        if (app.IsNull())
            throw new ArgumentNullException(nameof(app));

        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "1.0.0");

            if (streamHtml.Invoke() == null)
            {
                const string msg = "index.html属性错误";
                Log.Error(msg);
                throw new Exception(msg);
            }

            c.IndexStream = streamHtml;
            c.RoutePrefix = string.Empty;
        });
    }
}