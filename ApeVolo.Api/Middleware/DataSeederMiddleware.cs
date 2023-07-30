using System;
using ApeVolo.Common.ConfigOptions;
using ApeVolo.Common.Helper.Serilog;
using ApeVolo.Entity.Seed;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;

namespace ApeVolo.Api.Middleware;

public static class DataSeederMiddleware
{
    private static readonly ILogger Logger = SerilogManager.GetLogger(typeof(DataSeederMiddleware));

    public static void UseDataSeederMiddleware(this IApplicationBuilder app, DataContext dataContext)
    {
        if (app == null) throw new ArgumentNullException(nameof(app));

        try
        {
            var configs = app.ApplicationServices.GetRequiredService<IOptionsMonitor<Configs>>().CurrentValue;
            if (configs.IsInitTable)
            {
                DataSeeder.InitSystemDataAsync(dataContext, configs.IsInitData, configs.IsQuickDebug).Wait();
            }
        }
        catch (Exception e)
        {
            Logger.Error($"创建数据库初始化数据时错误.\n{e.Message}");
            throw;
        }
    }
}
