using System;
using Ape.Volo.Common.ConfigOptions;
using Ape.Volo.Common.Extensions;
using Ape.Volo.Common.Helper.Serilog;
using Ape.Volo.Entity.System;
using Ape.Volo.IBusiness.Interface.System;
using Ape.Volo.QuartzNetService.service;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;

namespace Ape.Volo.Api.Middleware;

/// <summary>
/// QuartzNet作业调度中间件
/// </summary>
public static class QuartzNetJobMiddleware
{
    private static readonly ILogger Logger = SerilogManager.GetLogger(typeof(QuartzNetJobMiddleware));

    public static void UseQuartzNetJobMiddleware(this IApplicationBuilder app, IQuartzNetService taskQuartzService,
        ISchedulerCenterService schedulerCenter)
    {
        if (app.IsNull())
            throw new ArgumentNullException(nameof(app));

        try
        {
            var configs = app.ApplicationServices.GetRequiredService<IOptionsMonitor<Configs>>().CurrentValue;
            if (configs.Middleware.QuartzNetJob.Enabled)
            {
                //var allTaskQuartzList = taskQuartzService.QueryAllAsync().Result;
                var allTaskQuartzList = taskQuartzService.SugarClient.Queryable<QuartzNet>().ToList();
                foreach (var item in allTaskQuartzList)
                {
                    if (!item.IsEnable) continue;
                    var results = schedulerCenter.AddScheduleJobAsync(item).Result;
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
