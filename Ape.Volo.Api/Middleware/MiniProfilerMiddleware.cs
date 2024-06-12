using System;
using Ape.Volo.Common.ConfigOptions;
using Ape.Volo.Common.Extensions;
using Ape.Volo.Common.Helper.Serilog;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;

namespace Ape.Volo.Api.Middleware;

/// <summary>
/// 性能监控中间件
/// </summary>
public static class MiniProfilerMiddleware
{
    private static readonly ILogger Logger = SerilogManager.GetLogger(typeof(MiniProfilerMiddleware));

    public static void UseMiniProfilerMiddleware(this IApplicationBuilder app)
    {
        if (app.IsNull())
            throw new ArgumentNullException(nameof(app));
        try
        {
            var configs = app.ApplicationServices.GetRequiredService<IOptionsMonitor<Configs>>().CurrentValue;
            if (configs.Middleware.MiniProfiler.Enabled)
            {
                // 性能分析
                app.UseMiniProfiler();
            }
        }
        catch (Exception e)
        {
            Logger.Error($"MiniProfilerMiddleware启动失败.\n{e.Message}");
            throw;
        }
    }
}
