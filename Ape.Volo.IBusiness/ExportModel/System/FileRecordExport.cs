using System.ComponentModel.DataAnnotations;
using Ape.Volo.Common.Model;

namespace Ape.Volo.IBusiness.ExportModel.System;

/// <summary>
/// 文件记录导出模板
/// </summary>
public class FileRecordExport : ExportBase
{
    /// <summary>
    /// 文件描述
    /// </summary>
    [Display(Name = "文件描述")]
    public string Description { get; set; }

    /// <summary>
    /// 文件类型
    /// </summary>
    [Display(Name = "文件类型")]
    public string ContentType { get; set; }

    /// <summary>
    /// 文件类型名称
    /// </summary>
    [Display(Name = "文件类型名称")]
    public string ContentTypeName { get; set; }

    /// <summary>
    /// 文件类型名称(EN)
    /// </summary>
    [Display(Name = "文件类型名称(EN)")]
    public string ContentTypeNameEn { get; set; }

    /// <summary>
    /// 源名称
    /// </summary>
    [Display(Name = "源名称")]
    public string OriginalName { get; set; }

    /// <summary>
    /// 新名称
    /// </summary>
    [Display(Name = "新名称")]
    public string NewName { get; set; }

    /// <summary>
    /// 存储路径
    /// </summary>
    [Display(Name = "存储路径")]
    public string FilePath { get; set; }

    /// <summary>
    /// 文件大小
    /// </summary>
    [Display(Name = "文件大小")]
    public string Size { get; set; }
}
