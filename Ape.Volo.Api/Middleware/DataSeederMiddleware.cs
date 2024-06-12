using System;
using System.Threading;
using Ape.Volo.Common.Helper;
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
                DataSeeder.InitMasterDataAsync(dataContext, dataContext.Configs.IsInitData,
                    dataContext.Configs.IsQuickDebug).Wait();
                Thread.Sleep(500); //保证顺序输出
                DataSeeder.InitLogData(dataContext);
                Thread.Sleep(500);
                DataSeeder.InitTenantDataAsync(dataContext).Wait();
            }
        }
        catch (Exception e)
        {
            Logger.Error($"创建数据库初始化数据时错误.\n{e.Message}");
            throw;
        }
    }
}
