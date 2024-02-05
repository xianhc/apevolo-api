using System.ComponentModel.DataAnnotations;
using Ape.Volo.Common.Model;

namespace Ape.Volo.IBusiness.ExportModel.System;

/// <summary>
/// 密钥导出模板
/// </summary>
public class AppSecretExport : ExportBase
{
    /// <summary>
    /// 应用ID
    /// </summary>
    [Display(Name = "应用ID")]
    public string AppId { get; set; }

    /// <summary>
    /// 应用密钥
    /// </summary>
    [Display(Name = "应用密钥")]
    public string AppSecretKey { get; set; }

    /// <summary>
    /// 应用名称
    /// </summary>
    [Display(Name = "应用名称")]
    public string AppName { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [Display(Name = "备注")]
    public string Remark { get; set; }
}
