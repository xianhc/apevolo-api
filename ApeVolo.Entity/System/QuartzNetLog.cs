using ApeVolo.Common.Model;
using ApeVolo.Entity.Base;
using SqlSugar;

namespace ApeVolo.Entity.System;

/// <summary>
/// 系统作业调度执行日志
/// </summary>
[SugarTable("sys_quartz_job_log", "系统作业调度执行日志")]
public class QuartzNetLog : BaseEntity, ISoftDeletedEntity
{
    /// <summary>
    /// 任务Id
    /// </summary>
    [SugarColumn(ColumnDescription = "任务名称")]
    public long TaskId { get; set; }

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
    /// cron 表达式
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "cron 表达式")]
    public string Cron { get; set; }

    /// <summary>
    /// 异常详情
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "异常详情")]
    public string ExceptionDetail { get; set; }

    /// <summary>
    /// 执行耗时（毫秒）
    /// </summary>
    [SugarColumn(ColumnDescription = "执行耗时(毫秒)")]
    public long ExecutionDuration { get; set; }

    /// <summary>
    /// 执行传参
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "执行传参")]
    public string RunParams { get; set; }

    /// <summary>
    /// 是否成功
    /// </summary>
    [SugarColumn(ColumnDescription = "是否成功")]
    public bool IsSuccess { get; set; }

    public bool IsDeleted { get; set; }
}
