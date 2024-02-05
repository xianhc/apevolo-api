using System.ComponentModel.DataAnnotations;
using Ape.Volo.Common.Global;
using Ape.Volo.Common.Model;

namespace Ape.Volo.IBusiness.ExportModel.System;

/// <summary>
/// 任务调度日志导出模板
/// </summary>
public class QuartzNetLogExport : ExportBase
{
    /// <summary>
    /// 任务ID
    /// </summary>
    [Display(Name = "任务ID")]
    public long TaskId { get; set; }

    /// <summary>
    /// 任务名称
    /// </summary>
    [Display(Name = "任务名称")]
    public string TaskName { get; set; }

    /// <summary>
    /// 任务组
    /// </summary>
    [Display(Name = "任务组")]
    public string TaskGroup { get; set; }

    /// <summary>
    /// 程序集名称
    /// </summary>
    [Display(Name = "程序集名称")]
    public string AssemblyName { get; set; }

    /// <summary>
    /// 执行类
    /// </summary>
    [Display(Name = "执行类")]
    public string ClassName { get; set; }

    /// <summary>
    /// Cron表达式
    /// </summary>
    [Display(Name = "Cron表达式")]
    public string Cron { get; set; }

    /// <summary>
    /// 异常详情
    /// </summary>
    [Display(Name = "异常详情")]
    public string ExceptionDetail { get; set; }

    /// <summary>
    /// 执行耗时
    /// </summary>
    [Display(Name = "执行耗时")]
    public long ExecutionDuration { get; set; }

    /// <summary>
    /// 执行传参
    /// </summary>
    [Display(Name = "执行传参")]
    public string RunParams { get; set; }

    /// <summary>
    /// 是否成功
    /// </summary>
    [Display(Name = "是否成功")]
    public BoolState IsSuccess { get; set; }
}
