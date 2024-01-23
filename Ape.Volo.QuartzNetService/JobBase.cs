using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Ape.Volo.Common.Extention;
using Ape.Volo.Entity.System;
using Ape.Volo.IBusiness.Interface.System;
using Ape.Volo.QuartzNetService.service;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Ape.Volo.QuartzNetService;

/// <summary>
/// 作业调度基类，所有作业须继承
/// </summary>
public class JobBase
{
    #region 字段

    public IQuartzNetService QuartzNetService;
    public IQuartzNetLogService QuartzNetLogService;
    public ISchedulerCenterService SchedulerCenterService;
    public ILogger<JobBase> Logger;

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
        var jobIdTmp = Convert.ToInt64(jobId);
        var quartzNet = await QuartzNetService.TableWhere(x => x.Id == jobIdTmp).SingleAsync();
        if (quartzNet == null)
        {
            await Task.CompletedTask;
            return;
        }

        //JOB组名
        string groupName = context.JobDetail.Key.Group;
        //日志
        string jobHistory =
            $"【{DateTime.Now:yyyy-MM-dd HH:mm:ss}】【执行开始】【组别：{groupName} => 任务：{quartzNet.TaskName} => Id：{jobId}】";
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
                await SchedulerCenterService.PauseJob(quartzNet);
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
            if (QuartzNetService != null)
            {
                quartzNet.RunTimes += 1;
                quartzNet.UpdateTime = DateTime.Now;
                quartzNet.UpdateBy = "QuartzNet Task";

                //记录任务日志
                var quartzNetLog = new QuartzNetLog
                {
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
                    CreateTime = quartzNet.UpdateTime ?? DateTime.MinValue
                };
                await QuartzNetService.UpdateJobInfoAsync(quartzNet, quartzNetLog);
            }
        }

        Logger.LogInformation(jobHistory);
    }

    #endregion
}
