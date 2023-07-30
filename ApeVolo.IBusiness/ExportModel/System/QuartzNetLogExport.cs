using System.ComponentModel.DataAnnotations;
using ApeVolo.Common.Global;
using ApeVolo.Common.Model;

namespace ApeVolo.IBusiness.ExportModel.System;

public class QuartzNetLogExport : ExportBase
{
    [Display(Name = "任务ID")]
    public long TaskId { get; set; }

    [Display(Name = "任务名称")]
    public string TaskName { get; set; }

    [Display(Name = "任务组")]
    public string TaskGroup { get; set; }

    [Display(Name = "程序集名称")]
    public string AssemblyName { get; set; }

    [Display(Name = "执行类")]
    public string ClassName { get; set; }

    [Display(Name = "Cron表达式")]
    public string Cron { get; set; }

    [Display(Name = "异常详情")]
    public string ExceptionDetail { get; set; }

    [Display(Name = "执行耗时")]
    public long ExecutionDuration { get; set; }

    [Display(Name = "执行传参")]
    public string RunParams { get; set; }

    [Display(Name = "是否成功")]
    public BoolState IsSuccess { get; set; }
}
