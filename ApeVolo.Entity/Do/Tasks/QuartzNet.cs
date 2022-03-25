using System;
using ApeVolo.Common.AttributeExt;
using SqlSugar;

namespace ApeVolo.Entity.Do.Tasks;

[InitTable(typeof(QuartzNet))]
[SugarTable("sys_quartz_job", "作业调度")]
public class QuartzNet : BaseEntity
{
    /// <summary>
    /// 任务名称
    /// </summary>
    [SugarColumn(ColumnName = "task_name", ColumnDataType = "varchar", Length = 255, ColumnDescription = "任务名称")]
    public string TaskName { get; set; }

    /// <summary>
    /// 任务分组
    /// </summary>
    [SugarColumn(ColumnName = "task_group", ColumnDataType = "varchar", Length = 255, ColumnDescription = "任务名称")]
    public string TaskGroup { get; set; }

    /// <summary>
    /// cron 表达式
    /// </summary>
    [SugarColumn(ColumnName = "cron", ColumnDataType = "varchar", Length = 255, IsNullable = true,
        ColumnDescription = "cron 表达式")]
    public string Cron { get; set; }

    /// <summary>
    /// 程序集名称
    /// </summary>
    [SugarColumn(ColumnName = "assembly_name", ColumnDataType = "varchar", Length = 255, ColumnDescription = "程序集名称")]
    public string AssemblyName { get; set; }

    /// <summary>
    /// 任务所在类
    /// </summary>
    [SugarColumn(ColumnName = "class_name", ColumnDataType = "varchar", Length = 255, ColumnDescription = "任务所在类")]
    public string ClassName { get; set; }

    /// <summary>
    /// 任务描述
    /// </summary>
    [SugarColumn(ColumnName = "description", ColumnDataType = "varchar", Length = 500, ColumnDescription = "任务描述")]
    public string Description { get; set; }

    /// <summary>
    /// 任务负责人
    /// </summary>
    [SugarColumn(ColumnName = "principal", ColumnDataType = "varchar", Length = 255, ColumnDescription = "任务负责人")]
    public string Principal { get; set; }

    /// <summary>
    /// 告警邮箱
    /// </summary>
    [SugarColumn(ColumnName = "alert_email", ColumnDataType = "varchar", Length = 255, IsNullable = true,
        ColumnDescription = "告警邮箱")]
    public string AlertEmail { get; set; }

    /// <summary>
    /// 任务失败后是否暂停
    /// </summary>
    [SugarColumn(ColumnName = "pause_after_failure", IsNullable = false, ColumnDescription = "任务失败后是否暂停")]
    public bool PauseAfterFailure { get; set; }

    /// <summary>
    /// 执行次数
    /// </summary>
    [SugarColumn(ColumnName = "run_times", ColumnDescription = "执行次数")]
    public int RunTimes { get; set; }

    /// <summary>
    /// 开始时间
    /// </summary>
    [SugarColumn(ColumnName = "start_time", IsNullable = true, ColumnDescription = "开始时间")]
    public DateTime? StartTime { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    [SugarColumn(ColumnName = "end_time", IsNullable = true, ColumnDescription = "结束时间")]
    public DateTime? EndTime { get; set; }

    /// <summary>
    /// 触发器类型（0、simple 1、cron）
    /// </summary>
    [SugarColumn(ColumnName = "trigger_type", ColumnDescription = "触发器类型（0、simple 1、cron）")]
    public int TriggerType { get; set; }

    /// <summary>
    /// 执行间隔时间, 秒为单位
    /// </summary>
    [SugarColumn(ColumnName = "interval_second", ColumnDescription = "执行间隔时间, 秒为单位")]
    public int IntervalSecond { get; set; }

    /// <summary>
    /// 循环执行次数
    /// </summary>
    [SugarColumn(ColumnName = "cycle_run_times", ColumnDescription = "循环执行次数")]
    public int CycleRunTimes { get; set; }

    /// <summary>
    /// 是否启动
    /// </summary>
    [SugarColumn(ColumnName = "is_enable", ColumnDescription = "是否启动")]
    public bool IsEnable { get; set; } = false;

    /// <summary>
    /// 执行传参
    /// </summary>
    [SugarColumn(ColumnName = "run_params", ColumnDataType = "varchar", Length = 255, IsNullable = true,
        ColumnDescription = "执行传参")]
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
}