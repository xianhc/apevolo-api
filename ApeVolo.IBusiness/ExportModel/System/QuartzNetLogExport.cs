using System;
using System.ComponentModel.DataAnnotations;
using ApeVolo.Common.Global;
using ApeVolo.Common.Model;

namespace ApeVolo.IBusiness.ExportModel.System;

public class QuartzNetLogExport : ExportBase
{
    [Display(Name = "TaskLog.TaskId")]
    public string TaskId { get; set; }

    [Display(Name = "Task.TaskName")]
    public string TaskName { get; set; }

    [Display(Name = "Task.TaskGroup")]
    public string TaskGroup { get; set; }

    [Display(Name = "Task.AssemblyName")]
    public string AssemblyName { get; set; }

    [Display(Name = "Task.ClassName")]
    public string ClassName { get; set; }

    [Display(Name = "Task.Cron")]
    public string Cron { get; set; }

    [Display(Name = "TaskLog.ExceptionDetail")]
    public string ExceptionDetail { get; set; }

    [Display(Name = "TaskLog.ExecutionDuration")]
    public long ExecutionDuration { get; set; }

    [Display(Name = "Task.RunParams")]
    public string RunParams { get; set; }

    [Display(Name = "TaskLog.IsSuccess")]
    public BoolState IsSuccess { get; set; }
}