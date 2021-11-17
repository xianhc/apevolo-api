using ApeVolo.Common.Extention;
using ApeVolo.Common.Global;
using log4net;
using Microsoft.AspNetCore.Builder;
using System;
using AspNetCoreRateLimit;

namespace ApeVolo.Api.Middleware
{
    /// <summary>
    /// IP限流策略中间件
    /// </summary>
    public static class IpLimitMiddleware
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(IpLimitMiddleware));

        public static void UseIpLimitMiddleware(this IApplicationBuilder app)
        {
            if (app.IsNull())
                throw new ArgumentNullException(nameof(app));

            try
            {
                if (AppSettings.GetValue("Middleware", "IpLimit", "Enabled").ToBool())
                {
                    app.UseIpRateLimiting();
                }
            }
            catch (Exception e)
            {
                Log.Error($"Error occured limiting ip rate.\n{e.Message}");
                throw;
            }
        }
    }
}