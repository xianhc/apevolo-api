using System.Threading.Tasks;
using ApeVolo.Entity.System.Task;
using ApeVolo.IBusiness.Dto.System.Task;

namespace ApeVolo.QuartzNetService.service;

/// <summary>
/// 作业调度接口
/// </summary>
public interface ISchedulerCenterService
{
    /// <summary>
    /// 开启任务调度
    /// </summary>
    /// <returns></returns>
    Task<bool> StartScheduleAsync();

    /// <summary>
    /// 停止任务调度
    /// </summary>
    /// <returns></returns>
    Task<bool> StopScheduleAsync();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="taskQuartz"></param>
    /// <returns></returns>
    Task<bool> AddScheduleJobAsync(QuartzNet taskQuartz);

    /// <summary>
    /// 停止一个任务
    /// </summary>
    /// <param name="taskQuartz"></param>
    /// <returns></returns>
    Task<bool> StopScheduleJobAsync(QuartzNet taskQuartz);

    /// <summary>
    /// 检测任务是否存在
    /// </summary>
    /// <param name="taskQuartz"></param>
    /// <returns></returns>
    Task<bool> IsExistScheduleJobAsync(QuartzNet taskQuartz);

    /// <summary>
    /// 暂停指定的计划任务
    /// </summary>
    /// <param name="taskQuartz"></param>
    /// <returns></returns>
    Task<bool> PauseJob(QuartzNet taskQuartz);

    /// <summary>
    /// 恢复一个任务
    /// </summary>
    /// <param name="taskQuartz"></param>
    /// <returns></returns>
    Task<bool> ResumeJob(QuartzNet taskQuartz);

    /// <summary>
    /// 获取任务触发器状态
    /// </summary>
    /// <param name="taskQuartz"></param>
    /// <returns></returns>
    Task<string> GetTriggerStatus(QuartzNetDto taskQuartzDto);

    /// <summary>
    /// 获取触发器标识
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    string GetTriggerState(string key);
}