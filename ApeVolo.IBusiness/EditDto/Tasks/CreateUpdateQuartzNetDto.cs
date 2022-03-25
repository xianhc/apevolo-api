using System;
using ApeVolo.Common.AttributeExt;
using ApeVolo.Common.Extention;
using ApeVolo.Entity.Do.Tasks;

namespace ApeVolo.IBusiness.EditDto.Tasks;

[AutoMapping(typeof(QuartzNet), typeof(CreateUpdateQuartzNetDto))]
public class CreateUpdateQuartzNetDto : BaseCreateUpdateEntityDto
{
    /// <summary>
    /// 任务名称
    /// </summary>
    public string TaskName { get; set; }

    /// <summary>
    /// 任务分组
    /// </summary>
    public string TaskGroup { get; set; }

    /// <summary>
    /// cron 表达式
    /// </summary>
    public string Cron { get; set; }

    /// <summary>
    /// 程序集名称
    /// </summary>
    public string AssemblyName { get; set; }

    /// <summary>
    /// 任务所在类
    /// </summary>
    public string ClassName { get; set; }

    /// <summary>
    /// 任务描述
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// 任务负责人
    /// </summary>
    public string Principal { get; set; }

    /// <summary>
    /// 告警邮箱
    /// </summary>
    public string AlertEmail { get; set; }

    /// <summary>
    /// 任务失败后是否继续
    /// </summary>
    public bool PauseAfterFailure { get; set; }

    /// <summary>
    /// 执行次数
    /// </summary>
    public int RunTimes { get; set; }

    /// <summary>
    /// 开始时间
    /// </summary>
    public DateTime? StartTime { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    public DateTime? EndTime { get; set; }

    /// <summary>
    /// 触发器类型（0、simple 1、cron）
    /// </summary>
    public int TriggerType => Cron.IsNullOrEmpty() ? 0 : 1;

    /// <summary>
    /// 执行间隔时间, 秒为单位
    /// </summary>
    public int IntervalSecond { get; set; }

    /// <summary>
    /// 循环执行次数
    /// </summary>
    public int CycleRunTimes { get; set; }

    /// <summary>
    /// 是否启动
    /// </summary>
    public bool IsEnable { get; set; } = false;

    /// <summary>
    /// 执行传参
    /// </summary>
    public string RunParams { get; set; }
}