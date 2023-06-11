using System.ComponentModel.DataAnnotations;
using ApeVolo.Common.Model;

namespace ApeVolo.IBusiness.ExportModel.System;

public class AppSecretExport : ExportBase
{
    [Display(Name = "App.AppId")]
    public string AppId { get; set; }

    [Display(Name = "App.AppSecretKey")]
    public string AppSecretKey { get; set; }

    [Display(Name = "App.AppName")]
    public string AppName { get; set; }

    [Display(Name = "App.Remark")]
    public string Remark { get; set; }
}