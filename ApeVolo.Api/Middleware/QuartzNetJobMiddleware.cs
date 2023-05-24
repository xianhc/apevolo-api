using System;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Global;
using ApeVolo.Common.Helper;
using ApeVolo.Common.Helper.Serilog;
using ApeVolo.IBusiness.Interface.System.Task;
using ApeVolo.QuartzNetService.service;
using Microsoft.AspNetCore.Builder;
using Serilog;

namespace ApeVolo.Api.Middleware;

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
            if (AppSettings.GetValue<bool>("Middleware", "QuartzNetJob", "Enabled"))
            {
                var allTaskQuartzList = taskQuartzService.QueryAllAsync().Result;
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