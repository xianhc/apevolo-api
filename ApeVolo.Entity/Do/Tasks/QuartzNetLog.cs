using ApeVolo.Common.DI;
using ApeVolo.Entity.Do.Base;
using SqlSugar;

namespace ApeVolo.Entity.Do.Tasks;

/// <summary>
/// 系统作业调度执行日志
/// </summary>
[SugarTable("sys_quartz_job_log", "系统作业调度执行日志")]
public class QuartzNetLog : EntityRoot<long>, ILocalizedTable
{
    /// <summary>
    /// 任务Id
    /// </summary>
    [SugarColumn(ColumnName = "task_id", ColumnDataType = "bigint", ColumnDescription = "任务名称")]
    public long TaskId { get; set; }

    /// <summary>
    /// 任务名称
    /// </summary>
    [SugarColumn(ColumnName = "task_name", ColumnDescription = "任务名称")]
    public string TaskName { get; set; }

    /// <summary>
    /// 任务分组
    /// </summary>
    [SugarColumn(ColumnName = "task_group", ColumnDescription = "任务名称")]
    public string TaskGroup { get; set; }

    /// <summary>
    /// 程序集名称
    /// </summary>
    [SugarColumn(ColumnName = "assembly_name", ColumnDescription = "程序集名称")]
    public string AssemblyName { get; set; }

    /// <summary>
    /// 任务所在类
    /// </summary>
    [SugarColumn(ColumnName = "class_name", ColumnDescription = "任务所在类")]
    public string ClassName { get; set; }

    /// <summary>
    /// cron 表达式
    /// </summary>
    [SugarColumn(ColumnName = "cron", IsNullable = true, ColumnDescription = "cron 表达式")]
    public string Cron { get; set; }

    /// <summary>
    /// 异常详情
    /// </summary>
    [SugarColumn(ColumnName = "exception_detail", ColumnDataType = "nvarchar", Length = 5000, IsNullable = true,
        ColumnDescription = "异常详情")]
    public string ExceptionDetail { get; set; }

    /// <summary>
    /// 执行耗时（毫秒）
    /// </summary>
    [SugarColumn(ColumnName = "execution_duration", ColumnDescription = "执行耗时(毫秒)")]
    public long ExecutionDuration { get; set; }

    /// <summary>
    /// 执行传参
    /// </summary>
    [SugarColumn(ColumnName = "run_params", IsNullable = true, ColumnDescription = "执行传参")]
    public string RunParams { get; set; }

    /// <summary>
    /// 是否成功
    /// </summary>
    [SugarColumn(ColumnName = "is_success", ColumnDescription = "是否成功")]
    public bool IsSuccess { get; set; }
}