using System.ComponentModel.DataAnnotations;
using ApeVolo.Common.Model;

namespace ApeVolo.IBusiness.ExportModel.System;

public class AppSecretExport : ExportBase
{
    [Display(Name = "应用ID")]
    public string AppId { get; set; }

    [Display(Name = "应用密钥")]
    public string AppSecretKey { get; set; }

    [Display(Name = "应用名称")]
    public string AppName { get; set; }

    [Display(Name = "备注")]
    public string Remark { get; set; }
}
