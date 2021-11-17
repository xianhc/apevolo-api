using ApeVolo.Common.Extention;
using ApeVolo.Common.Global;
using Microsoft.AspNetCore.Builder;
using log4net;
using System;

namespace ApeVolo.Api.Middleware
{
    /// <summary>
    /// 性能监控中间件
    /// </summary>
    public static class MiniProfilerMiddleware
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(MiniProfilerMiddleware));

        public static void UseMiniProfilerMiddleware(this IApplicationBuilder app)
        {
            if (app.IsNull())
                throw new ArgumentNullException(nameof(app));

            try
            {
                if (AppSettings.GetValue("Middleware", "MiniProfiler", "Enabled").ToBool())
                {
                    // 性能分析
                    app.UseMiniProfiler();
                }
            }
            catch (Exception e)
            {
                Log.Error($"MiniProfilerMiddleware启动失败.\n{e.Message}");
                throw;
            }
        }
    }
}