using System;
using ApeVolo.Common.Model;
using ApeVolo.Entity.Base;
using SqlSugar;

namespace ApeVolo.Entity.System;

/// <summary>
/// 系统作业调度
/// </summary>
[SugarTable("sys_quartz_job")]
public class QuartzNet : BaseEntity, ISoftDeletedEntity
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
    [SugarColumn(IsNullable = true)]
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
    [SugarColumn(Length = 500, IsNullable = true)]
    public string Description { get; set; }

    /// <summary>
    /// 任务负责人
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public string Principal { get; set; }

    /// <summary>
    /// 告警邮箱
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public string AlertEmail { get; set; }

    /// <summary>
    /// 任务失败后是否暂停
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public bool PauseAfterFailure { get; set; }

    /// <summary>
    /// 执行次数
    /// </summary>
    public int RunTimes { get; set; }

    /// <summary>
    /// 开始时间
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public DateTime? StartTime { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public DateTime? EndTime { get; set; }

    /// <summary>
    /// 触发器类型（0、simple 1、cron）
    /// </summary>
    public int TriggerType { get; set; }

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
    [SugarColumn(IsNullable = true)]
    public string RunParams { get; set; }

    /// <summary>
    /// 触发器状态
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public string TriggerStatus { get; set; }

    /// <summary>
    /// 触发器模式
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public string TriggerTypeStr => TriggerType == 1 ? "cron" : "simple";

    /// <summary>
    /// 是否已删除
    /// </summary>
    public bool IsDeleted { get; set; }
}
