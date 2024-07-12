using System;
using Ape.Volo.Common;
using Ape.Volo.Common.ConfigOptions;
using Ape.Volo.Common.Extensions;
using Ape.Volo.Common.Helper;
using Ape.Volo.Common.Helper.Serilog;
using Ape.Volo.IBusiness.Interface.System;
using Ape.Volo.QuartzNetService.service;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Ape.Volo.Api.Middleware;

/// <summary>
/// QuartzNet作业调度中间件
/// </summary>
public static class QuartzNetJobMiddleware
{
    private static readonly ILogger Logger = SerilogManager.GetLogger(typeof(QuartzNetJobMiddleware));

    public static void UseQuartzNetJobMiddleware(this IApplicationBuilder app)
    {
        if (app.IsNull())
            throw new ArgumentNullException(nameof(app));

        try
        {
            if (App.GetOptions<MiddlewareOptions>().QuartzNetJob.Enabled)
            {
                var quartzNetService = app.ApplicationServices.GetRequiredService<IQuartzNetService>();
                var schedulerCenter = app.ApplicationServices.GetRequiredService<ISchedulerCenterService>();
                var allTaskQuartzList = AsyncHelper.RunSync(() => quartzNetService.QueryAllAsync());
                //var allTaskQuartzList = quartzNetService.SugarClient.Queryable<QuartzNet>().ToList();
                foreach (var item in allTaskQuartzList)
                {
                    if (!item.IsEnable) continue;
                    var results = AsyncHelper.RunSync(() => schedulerCenter.AddScheduleJobAsync(item));
                    Logger.Information(results ? $"作业=>{item.TaskName}=>启动成功！" : $"作业=>{item.TaskName}=>启动失败！");
                }
            }
        }
        catch (Exception e)
        {
            Logger.Error($"启动作业调度服务失败。\n{e.Message}");
            throw;
        }
    }
}
