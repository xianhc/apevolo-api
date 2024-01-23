using System;
using Ape.Volo.Common.Helper.Serilog;
using Ape.Volo.Entity.Seed;
using Microsoft.AspNetCore.Builder;
using Serilog;

namespace Ape.Volo.Api.Middleware;

public static class DataSeederMiddleware
{
    private static readonly ILogger Logger = SerilogManager.GetLogger(typeof(DataSeederMiddleware));

    public static void UseDataSeederMiddleware(this IApplicationBuilder app, DataContext dataContext)
    {
        if (app == null) throw new ArgumentNullException(nameof(app));

        try
        {
            if (dataContext.Configs.IsInitTable)
            {
                DataSeeder.InitSystemDataAsync(dataContext, dataContext.Configs.IsInitData,
                    dataContext.Configs.IsQuickDebug).Wait();
            }
        }
        catch (Exception e)
        {
            Logger.Error($"创建数据库初始化数据时错误.\n{e.Message}");
            throw;
        }
    }
}
