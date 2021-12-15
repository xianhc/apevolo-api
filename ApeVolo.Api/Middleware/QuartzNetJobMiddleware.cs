using System;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Global;
using ApeVolo.IBusiness.Interface.Tasks;
using ApeVolo.QuartzNetService.service;
using log4net;
using Microsoft.AspNetCore.Builder;

namespace ApeVolo.Api.Middleware
{
    /// <summary>
    /// QuartzNet作业调度中间件
    /// </summary>
    public static class QuartzNetJobMiddleware
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(QuartzNetJobMiddleware));

        public static void UseQuartzNetJobMiddleware(this IApplicationBuilder app, IQuartzNetService taskQuartzService,
            ISchedulerCenterService schedulerCenter)
        {
            if (app.IsNull())
                throw new ArgumentNullException(nameof(app));

            try
            {
                if (AppSettings.GetValue("Middleware", "QuartzNetJob", "Enabled").ToBool())
                {
                    var allTaskQuartzList = taskQuartzService.QueryAllAsync().Result;
                    foreach (var item in allTaskQuartzList)
                    {
                        if (!item.IsEnable) continue;
                        var results = schedulerCenter.AddScheduleJobAsync(item).Result;
                        Log.Error(results ? $"作业=>{item.TaskName}=>启动成功！" : $"作业=>{item.TaskName}=>启动失败！");
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error($"启动作业调度服务失败。\n{e.Message}");
                throw;
            }
        }
    }
}