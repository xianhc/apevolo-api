using System;
using System.Collections.Specialized;
using System.Reflection;
using System.Threading.Tasks;
using Ape.Volo.Common.Enums;
using Ape.Volo.Common.Helper.Serilog;
using Ape.Volo.Entity.System;
using Ape.Volo.IBusiness.Dto.System;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Triggers;
using Quartz.Spi;
using Serilog;

namespace Ape.Volo.QuartzNetService.service;

/// <summary>
/// 作业调度接口
/// </summary>
public class SchedulerCenterService : ISchedulerCenterService
{
    private static readonly ILogger Logger = SerilogManager.GetLogger(typeof(SchedulerCenterService));
    private Task<IScheduler> _scheduler;
    private readonly IJobFactory _iocjobFactory;

    public SchedulerCenterService(IJobFactory jobFactory)
    {
        _iocjobFactory = jobFactory;
        _scheduler = GetSchedulerAsync();
    }

    private Task<IScheduler> GetSchedulerAsync()
    {
        if (_scheduler != null)
            return _scheduler;
        // 从Factory中获取Scheduler实例
        NameValueCollection collection = new NameValueCollection
        {
            { "quartz.serializer.type", "binary" },
        };
        StdSchedulerFactory factory = new StdSchedulerFactory(collection);
        return _scheduler = factory.GetScheduler();
    }

    /// <summary>
    /// 开启任务
    /// </summary>
    /// <returns></returns>
    public async Task<bool> StartScheduleAsync()
    {
        try
        {
            _scheduler.Result.JobFactory = _iocjobFactory;
            if (!_scheduler.Result.IsStarted)
            {
                await _scheduler.Result.Start();
                return true;
            }
        }
        catch (Exception ex)
        {
            Logger.Error(ex.Message + "\n" + ex.StackTrace);
        }

        return false;
    }

    /// <summary>
    /// 停止任务
    /// </summary>
    /// <returns></returns>
    public async Task<bool> ShutdownScheduleAsync()
    {
        try
        {
            if (!_scheduler.Result.IsShutdown)
            {
                await _scheduler.Result.Shutdown();
                return true;
            }
        }
        catch (Exception ex)
        {
            Logger.Error(ex.Message + "\n" + ex.StackTrace);
        }

        return false;
    }

    /// <summary>
    /// 添加任务
    /// </summary>
    /// <param name="taskQuartz"></param>
    /// <returns></returns>
    public async Task<bool> AddScheduleJobAsync(QuartzNet taskQuartz)
    {
        if (taskQuartz != null)
        {
            try
            {
                JobKey jobKey = new JobKey(taskQuartz.Id.ToString(), taskQuartz.TaskGroup);
                if (await _scheduler.Result.CheckExists(jobKey))
                {
                    return true;
                }


                #region 通过反射获取程序集类型和类

                Assembly assembly = Assembly.Load(new AssemblyName(taskQuartz.AssemblyName));
                Type jobType = assembly.GetType(taskQuartz.AssemblyName + "." + taskQuartz.ClassName);

                #endregion

                //判断任务调度是否开启
                if (!_scheduler.Result.IsStarted)
                {
                    await StartScheduleAsync();
                }

                //传入反射出来的执行程序集
                if (jobType != null)
                {
                    IJobDetail job = new JobDetailImpl(taskQuartz.Id.ToString(), taskQuartz.TaskGroup, jobType);
                    job.JobDataMap.Add("JobParam", taskQuartz.RunParams);
                    ITrigger trigger;


                    if (taskQuartz.TriggerType == TriggerType.Cron)
                    {
                        trigger = CreateCronTrigger(taskQuartz);

                        ((CronTriggerImpl)trigger).MisfireInstruction = MisfireInstruction.CronTrigger.DoNothing;
                    }
                    else
                    {
                        trigger = CreateSimpleTrigger(taskQuartz);
                    }

                    // 开启作业
                    await _scheduler.Result.ScheduleJob(job, trigger);

                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message + "\n" + ex.StackTrace);
            }
        }

        return false;
    }

    /// <summary>
    /// 删除任务
    /// </summary>
    /// <returns></returns>
    public async Task<bool> DeleteScheduleJobAsync(QuartzNet taskQuartz)
    {
        try
        {
            JobKey jobKey = new JobKey(taskQuartz.Id.ToString(), taskQuartz.TaskGroup);
            if (await _scheduler.Result.CheckExists(jobKey))
            {
                await _scheduler.Result.DeleteJob(jobKey);
            }
        }
        catch (Exception ex)
        {
            Logger.Error(ex.Message + "\n" + ex.StackTrace);
            return false;
        }

        return true;
    }

    /// <summary>
    /// 恢复任务
    /// </summary>
    /// <param name="taskQuartz"></param>
    /// <returns></returns>
    public async Task<bool> ResumeJob(QuartzNet taskQuartz)
    {
        try
        {
            JobKey jobKey = new JobKey(taskQuartz.Id.ToString(), taskQuartz.TaskGroup);
            if (await _scheduler.Result.CheckExists(jobKey))
            {
                await _scheduler.Result.ResumeJob(jobKey);
                return true;
            }
        }
        catch (Exception ex)
        {
            Logger.Error(ex.Message + "\n" + ex.StackTrace);
        }

        return false;
    }

    /// <summary>
    /// 暂停任务
    /// </summary>
    /// <param name="taskQuartz"></param>
    /// <returns></returns>
    public async Task<bool> PauseJob(QuartzNet taskQuartz)
    {
        try
        {
            JobKey jobKey = new JobKey(taskQuartz.Id.ToString(), taskQuartz.TaskGroup);
            if (await _scheduler.Result.CheckExists(jobKey))
            {
                await _scheduler.Result.PauseJob(jobKey);
                return true;
            }
        }
        catch (Exception ex)
        {
            Logger.Error(ex.Message + "\n" + ex.StackTrace);
        }

        return false;
    }

    /// <summary>
    /// 检测任务是否存在
    /// </summary>
    /// <returns></returns>
    public async Task<bool> IsExistScheduleJobAsync(QuartzNet taskQuartz)
    {
        JobKey jobKey = new JobKey(taskQuartz.Id.ToString(), taskQuartz.TaskGroup);
        return await _scheduler.Result.CheckExists(jobKey);
    }


    /// <summary>
    /// 获取任务触发器状态
    /// </summary>
    /// <param name="taskQuartzDto"></param>
    /// <returns></returns>
    public async Task<string> GetTriggerStatus(QuartzNetDto taskQuartzDto)
    {
        string triggerStatus = "未执行";
        JobKey jobKey = new JobKey(taskQuartzDto.Id.ToString(), taskQuartzDto.TaskGroup);
        IJobDetail job = await _scheduler.Result.GetJobDetail(jobKey);
        if (job == null)
        {
            return triggerStatus;
        }

        var triggers = await _scheduler.Result.GetTriggersOfJob(jobKey);
        if (triggers.Count == 0)
        {
            return triggerStatus;
        }

        foreach (var trigger in triggers)
        {
            if (((JobDetailImpl)job).Name == ((AbstractTrigger)trigger).Name)
            {
                var state = await _scheduler.Result.GetTriggerState(trigger.Key);
                triggerStatus = state switch
                {
                    TriggerState.Blocked => "阻塞",
                    TriggerState.Complete => "完成",
                    TriggerState.Error => "错误",
                    TriggerState.None => "阻塞",
                    TriggerState.Normal => "运行中",
                    TriggerState.Paused => "暂停",
                    _ => triggerStatus
                };
            }
        }

        return triggerStatus;
    }


    #region 创建触发器

    /// <summary>
    /// 创建SimpleTrigger触发器
    /// </summary>
    /// <param name="taskQuartz"></param>
    /// <returns></returns>
    private ITrigger CreateSimpleTrigger(QuartzNet taskQuartz)
    {
        taskQuartz.StartTime ??= DateTime.Now;
        var startAt = DateBuilder.NextGivenSecondDate(taskQuartz.StartTime, 1); //设置开始时间
        taskQuartz.EndTime ??= DateTime.MaxValue.AddDays(-1);
        var endAt = DateBuilder.NextGivenSecondDate(taskQuartz.EndTime, 1); //设置暂停时间
        if (taskQuartz.CycleRunTimes > 0)
        {
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity(taskQuartz.Id.ToString(), taskQuartz.TaskGroup)
                .StartAt(startAt)
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(taskQuartz.IntervalSecond)
                    .WithRepeatCount(taskQuartz.CycleRunTimes - 1))
                .EndAt(endAt)
                .Build();
            return trigger;
        }
        else
        {
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity(taskQuartz.Id.ToString(), taskQuartz.TaskGroup)
                .StartAt(startAt)
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(taskQuartz.IntervalSecond)
                    .RepeatForever()
                )
                .EndAt(endAt)
                .Build();
            return trigger;
        }
    }

    /// <summary>
    /// 创建类型Cron的触发器
    /// </summary>
    /// <param name="taskQuartz"></param>
    /// <returns></returns>
    private ITrigger CreateCronTrigger(QuartzNet taskQuartz)
    {
        taskQuartz.StartTime ??= DateTime.Now;
        var startAt = DateBuilder.NextGivenSecondDate(taskQuartz.StartTime, 1); //设置开始时间
        taskQuartz.EndTime ??= DateTime.MaxValue.AddDays(-1);
        var endAt = DateBuilder.NextGivenSecondDate(taskQuartz.EndTime, 1); //设置暂停时间


        return TriggerBuilder.Create()
            .WithIdentity(taskQuartz.Id.ToString(), taskQuartz.TaskGroup)
            .StartAt(startAt)
            .EndAt(endAt)
            .WithCronSchedule(taskQuartz.Cron)
            .ForJob(taskQuartz.Id.ToString(), taskQuartz.TaskGroup)
            .Build();
    }

    #endregion
}
