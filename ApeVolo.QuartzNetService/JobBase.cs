using System;
using System.Diagnostics;
using System.Threading.Tasks;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Helper;
using ApeVolo.Common.SnowflakeIdHelper;
using ApeVolo.Entity.Do.Tasks;
using ApeVolo.IBusiness.Interface.Tasks;
using ApeVolo.QuartzNetService.service;
using Quartz;

namespace ApeVolo.QuartzNetService;

/// <summary>
/// 作业调度基类，所有作业须继承
/// </summary>
public class JobBase
{
    #region 字段

    public IQuartzNetService _quartzNetService;
    public IQuartzNetLogService _quartzNetLogService;
    public ISchedulerCenterService _schedulerCenterService;

    #endregion

    #region 执行方法

    /// <summary>
    /// 执行指定任务
    /// </summary>
    /// <param name="context"></param>
    /// <param name="func"></param>
    protected async Task ExecuteJob(IJobExecutionContext context, Func<Task> func)
    {
        //是否成功
        bool isSucceed = true;
        //异常详情
        string exceptionDetail = string.Empty;
        //记录Job时间
        Stopwatch stopwatch = new Stopwatch();
        //JOBID
        string jobId = context.JobDetail.Key.Name;
        var quartzNet = await _quartzNetService.QuerySingleAsync(jobId);
        //JOB组名
        string groupName = context.JobDetail.Key.Group;
        //日志
        string jobHistory = $"【{DateTime.Now:yyyy-MM-dd HH:mm:ss}】【执行开始】【组别：{groupName} => Id：{jobId}】";
        //耗时
        try
        {
            stopwatch.Start();
            await func(); //执行任务
            stopwatch.Stop();
            jobHistory += $"，【{DateTime.Now:yyyy-MM-dd HH:mm:ss}】【执行成功】";
        }
        catch (Exception ex)
        {
            isSucceed = false;
            exceptionDetail = $" {ex.Message}\n{ex.StackTrace}";
            JobExecutionException e2 = new JobExecutionException(ex);
            //失败后是否暂停
            if (quartzNet.PauseAfterFailure)
            {
                await _schedulerCenterService.PauseJob(quartzNet);
            }
            else
            {
                //true  是立即重新执行任务 
                e2.RefireImmediately = true;
            }

            //告警邮箱
            if (!quartzNet.AlertEmail.IsNullOrEmpty())
            {
                //实现自己的告警邮件模板 再添加队列信息
            }

            jobHistory += $"，【{DateTime.Now:yyyy-MM-dd HH:mm:ss}】【执行失败:{ex.Message}】";
        }
        finally
        {
            var taskSeconds = Math.Round(stopwatch.Elapsed.TotalSeconds, 3);
            jobHistory += $"，【{DateTime.Now:yyyy-MM-dd HH:mm:ss}】【执行结束】(耗时:{taskSeconds}秒)";
            if (_quartzNetService != null)
            {
                if (quartzNet != null)
                {
                    quartzNet.RunTimes += 1;
                    quartzNet.UpdateTime = DateTime.Now;
                    quartzNet.UpdateBy = "QuartzNet Task";

                    //记录任务日志
                    QuartzNetLog quartzNetLog = new QuartzNetLog
                    {
                        Id = IdHelper.GetLongId(),
                        TaskId = quartzNet.Id,
                        TaskName = quartzNet.TaskName,
                        TaskGroup = quartzNet.TaskGroup,
                        AssemblyName = quartzNet.AssemblyName,
                        ClassName = quartzNet.ClassName,
                        Cron = quartzNet.Cron,
                        ExceptionDetail = exceptionDetail,
                        ExecutionDuration = stopwatch.ElapsedMilliseconds,
                        RunParams = quartzNet.RunParams,
                        IsSuccess = isSucceed,
                        CreateBy = "QuartzNet Task",
                        CreateTime = quartzNet.UpdateTime
                    };
                    await _quartzNetService.UpdateJobInfoAsync(quartzNet, quartzNetLog);
                }
            }
        }

        LogHelper.WriteLog(jobHistory,
            new[] { "QuartzInfo", quartzNet?.TaskGroup + "_" + quartzNet?.TaskName });
    }

    #endregion
}