using System;
using System.Collections.Specialized;
using System.Reflection;
using System.Threading.Tasks;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Helper;
using ApeVolo.Entity.System.Task;
using ApeVolo.IBusiness.Dto.System.Task;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Triggers;
using Quartz.Spi;

namespace ApeVolo.QuartzNetService.service;

/// <summary>
/// 作业调度接口
/// </summary>
public class SchedulerCenterService : ISchedulerCenterService
{
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
    /// 开启任务调度
    /// </summary>
    /// <returns></returns>
    public async Task<bool> StartScheduleAsync()
    {
        var isTrue = true;
        try
        {
            _scheduler.Result.JobFactory = _iocjobFactory;
            if (!_scheduler.Result.IsStarted)
            {
                //等待任务运行完成
                await _scheduler.Result.Start();
                return isTrue;
            }

            isTrue = false;
        }
        catch (Exception ex)
        {
            LogHelper.WriteError(ex.Message + "\n" + ex.StackTrace, new[] { "QuartzException" });
            isTrue = false;
        }

        return isTrue;
    }

    /// <summary>
    /// 停止任务调度
    /// </summary>
    /// <returns></returns>
    public async Task<bool> StopScheduleAsync()
    {
        var isTrue = true;
        try
        {
            if (!_scheduler.Result.IsShutdown)
            {
                //等待任务运行完成
                await _scheduler.Result.Shutdown();
                return isTrue;
            }

            isTrue = false;
        }
        catch (Exception ex)
        {
            LogHelper.WriteError(ex.Message + "\n" + ex.StackTrace, new[] { "QuartzException" });
            isTrue = false;
        }

        return isTrue;
    }

    /// <summary>
    /// 添加一个计划任务
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="taskQuartz"></param>
    /// <returns></returns>
    public async Task<bool> AddScheduleJobAsync(QuartzNet taskQuartz)
    {
        var isTrue = true;
        if (taskQuartz != null)
        {
            try
            {
                JobKey jobKey = new JobKey(taskQuartz.Id.ToString(), taskQuartz.TaskGroup);
                if (await _scheduler.Result.CheckExists(jobKey))
                {
                    return isTrue;
                }

                #region 设置作业时间

                if (taskQuartz.StartTime == null)
                {
                    taskQuartz.StartTime = DateTime.Now;
                }

                DateTimeOffset starRunTime = DateBuilder.NextGivenSecondDate(taskQuartz.StartTime, 1); //设置开始时间
                if (taskQuartz.EndTime == null)
                {
                    taskQuartz.EndTime = DateTime.MaxValue.AddDays(-1);
                }

                DateTimeOffset endRunTime = DateBuilder.NextGivenSecondDate(taskQuartz.EndTime, 1); //设置暂停时间

                #endregion

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
                IJobDetail job = new JobDetailImpl(taskQuartz.Id.ToString(), taskQuartz.TaskGroup, jobType);
                job.JobDataMap.Add("JobParam", taskQuartz.RunParams);
                ITrigger trigger;


                if (!taskQuartz.Cron.IsNullOrEmpty() && CronExpression.IsValidExpression(taskQuartz.Cron))
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
                return isTrue;
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex.Message + "\n" + ex.StackTrace, new[] { "QuartzException" });
                isTrue = false;
            }
        }
        else
        {
            isTrue = false;
        }

        return isTrue;
    }

    /// <summary>
    /// 任务是否存在?
    /// </summary>
    /// <returns></returns>
    public async Task<bool> IsExistScheduleJobAsync(QuartzNet taskQuartz)
    {
        JobKey jobKey = new JobKey(taskQuartz.Id.ToString(), taskQuartz.TaskGroup);
        return await _scheduler.Result.CheckExists(jobKey) ? true : false;
    }

    /// <summary>
    /// 暂停一个指定的计划任务
    /// </summary>
    /// <returns></returns>
    public async Task<bool> StopScheduleJobAsync(QuartzNet taskQuartz)
    {
        var isTrue = true;
        try
        {
            JobKey jobKey = new JobKey(taskQuartz.Id.ToString(), taskQuartz.TaskGroup);
            if (await _scheduler.Result.CheckExists(jobKey))
            {
                await _scheduler.Result.DeleteJob(jobKey);
                return isTrue;
            }
        }
        catch (Exception ex)
        {
            LogHelper.WriteError(ex.Message + "\n" + ex.StackTrace, new[] { "QuartzException" });
            isTrue = false;
        }

        return isTrue;
    }

    /// <summary>
    /// 恢复指定的计划任务
    /// </summary>
    /// <param name="taskQuartz"></param>
    /// <returns></returns>
    public async Task<bool> ResumeJob(QuartzNet taskQuartz)
    {
        var isTrue = true;
        try
        {
            JobKey jobKey = new JobKey(taskQuartz.Id.ToString(), taskQuartz.TaskGroup);
            if (await _scheduler.Result.CheckExists(jobKey))
            {
                await _scheduler.Result.ResumeJob(jobKey);
                return isTrue;
            }
        }
        catch (Exception ex)
        {
            LogHelper.WriteError(ex.Message + "\n" + ex.StackTrace, new[] { "QuartzException" });
            isTrue = false;
        }

        return isTrue;
    }

    /// <summary>
    /// 暂停指定的计划任务
    /// </summary>
    /// <param name="taskQuartz"></param>
    /// <returns></returns>
    public async Task<bool> PauseJob(QuartzNet taskQuartz)
    {
        var isTrue = true;
        try
        {
            JobKey jobKey = new JobKey(taskQuartz.Id.ToString(), taskQuartz.TaskGroup);
            if (await _scheduler.Result.CheckExists(jobKey))
            {
                await _scheduler.Result.PauseJob(jobKey);
                return isTrue;
            }
        }
        catch (Exception ex)
        {
            LogHelper.WriteError(ex.Message + "\n" + ex.StackTrace, new[] { "QuartzException" });
            isTrue = false;
        }

        return isTrue;
    }

    #region Quartz状态

    public async Task<string> GetTriggerStatus(QuartzNetDto taskQuartzDto)
    {
        string triggerStatus = "不存在";
        JobKey jobKey = new JobKey(taskQuartzDto.Id.ToString(), taskQuartzDto.TaskGroup);
        IJobDetail job = await _scheduler.Result.GetJobDetail(jobKey);
        if (job == null)
        {
            return triggerStatus;
        }

        var triggers = await _scheduler.Result.GetTriggersOfJob(jobKey);
        if (triggers == null || triggers.Count == 0)
        {
            return triggerStatus;
        }

        foreach (var trigger in triggers)
        {
            if (((JobDetailImpl)job).Name == ((AbstractTrigger)trigger).Name)
            {
                var tStatus = await _scheduler.Result.GetTriggerState(trigger.Key);
                triggerStatus = GetTriggerState(tStatus.ToString());
                break;
            }
        }

        return triggerStatus;
    }

    #endregion

    #region GetTriggerState

    public string GetTriggerState(string key)
    {
        string state = null;
        if (key != null)
            key = key.ToUpper();
        switch (key)
        {
            case "1":
                state = "暂停";
                break;
            case "2":
                state = "完成";
                break;
            case "3":
                state = "出错";
                break;
            case "4":
                state = "阻塞";
                break;
            case "0":
                state = "正常";
                break;
            case "-1":
                state = "不存在";
                break;
            case "BLOCKED":
                state = "阻塞";
                break;
            case "COMPLETE":
                state = "完成";
                break;
            case "ERROR":
                state = "出错";
                break;
            case "NONE":
                state = "不存在";
                break;
            case "NORMAL":
                state = "正常";
                break;
            case "PAUSED":
                state = "暂停";
                break;
        }

        return state;
    }

    #endregion

    #region 创建触发器

    /// <summary>
    /// 创建SimpleTrigger触发器
    /// </summary>
    /// <param name="taskQuartz"></param>
    /// <returns></returns>
    private ITrigger CreateSimpleTrigger(QuartzNet taskQuartz)
    {
        if (taskQuartz.CycleRunTimes > 0)
        {
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity(taskQuartz.Id.ToString(), taskQuartz.TaskGroup)
                .StartAt(taskQuartz.StartTime.Value)
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(taskQuartz.IntervalSecond)
                    .WithRepeatCount(taskQuartz.CycleRunTimes - 1))
                .EndAt(taskQuartz.EndTime)
                .Build();
            return trigger;
        }
        else
        {
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity(taskQuartz.Id.ToString(), taskQuartz.TaskGroup)
                .StartAt(taskQuartz.StartTime.Value)
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(taskQuartz.IntervalSecond)
                    .RepeatForever()
                )
                .EndAt(taskQuartz.EndTime)
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
        // 作业触发器
        return TriggerBuilder.Create()
            .WithIdentity(taskQuartz.Id.ToString(), taskQuartz.TaskGroup)
            .StartAt(taskQuartz.StartTime.Value) //开始时间
            .EndAt(taskQuartz.EndTime.Value) //结束数据
            .WithCronSchedule(taskQuartz.Cron) //指定cron表达式
            .ForJob(taskQuartz.Id.ToString(), taskQuartz.TaskGroup) //作业名称
            .Build();
    }

    #endregion
}