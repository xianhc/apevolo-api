using System;
using ApeVolo.Common.Model;
using ApeVolo.Entity.Base;
using SqlSugar;

namespace ApeVolo.Entity.System;

/// <summary>
/// 系统作业调度
/// </summary>
[SugarTable("sys_quartz_job", "系统作业调度")]
public class QuartzNet : BaseEntity, ISoftDeletedEntity
{
    /// <summary>
    /// 任务名称
    /// </summary>
    [SugarColumn(ColumnDescription = "任务名称")]
    public string TaskName { get; set; }

    /// <summary>
    /// 任务分组
    /// </summary>
    [SugarColumn(ColumnDescription = "任务名称")]
    public string TaskGroup { get; set; }

    /// <summary>
    /// cron 表达式
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "cron 表达式")]
    public string Cron { get; set; }

    /// <summary>
    /// 程序集名称
    /// </summary>
    [SugarColumn(ColumnDescription = "程序集名称")]
    public string AssemblyName { get; set; }

    /// <summary>
    /// 任务所在类
    /// </summary>
    [SugarColumn(ColumnDescription = "任务所在类")]
    public string ClassName { get; set; }

    /// <summary>
    /// 任务描述
    /// </summary>
    [SugarColumn(Length = 500, ColumnDescription = "任务描述")]
    public string Description { get; set; }

    /// <summary>
    /// 任务负责人
    /// </summary>
    [SugarColumn(ColumnDescription = "任务负责人")]
    public string Principal { get; set; }

    /// <summary>
    /// 告警邮箱
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "告警邮箱")]
    public string AlertEmail { get; set; }

    /// <summary>
    /// 任务失败后是否暂停
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "任务失败后是否暂停")]
    public bool PauseAfterFailure { get; set; }

    /// <summary>
    /// 执行次数
    /// </summary>
    [SugarColumn(ColumnDescription = "执行次数")]
    public int RunTimes { get; set; }

    /// <summary>
    /// 开始时间
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "开始时间")]
    public DateTime? StartTime { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "结束时间")]
    public DateTime? EndTime { get; set; }

    /// <summary>
    /// 触发器类型（0、simple 1、cron）
    /// </summary>
    [SugarColumn(ColumnDescription = "触发器类型（0、simple 1、cron）")]
    public int TriggerType { get; set; }

    /// <summary>
    /// 执行间隔时间, 秒为单位
    /// </summary>
    [SugarColumn(ColumnDescription = "执行间隔时间, 秒为单位")]
    public int IntervalSecond { get; set; }

    /// <summary>
    /// 循环执行次数
    /// </summary>
    [SugarColumn(ColumnDescription = "循环执行次数")]
    public int CycleRunTimes { get; set; }

    /// <summary>
    /// 是否启动
    /// </summary>
    [SugarColumn(ColumnDescription = "是否启动")]
    public bool IsEnable { get; set; } = false;

    /// <summary>
    /// 执行传参
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "执行传参")]
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

    public bool IsDeleted { get; set; }
}
