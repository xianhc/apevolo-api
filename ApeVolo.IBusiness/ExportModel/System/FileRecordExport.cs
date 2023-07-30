using System.ComponentModel.DataAnnotations;
using ApeVolo.Common.Model;

namespace ApeVolo.IBusiness.ExportModel.System;

public class FileRecordExport : ExportBase
{
    [Display(Name = "文件描述")]
    public string Description { get; set; }

    [Display(Name = "文件类型")]
    public string ContentType { get; set; }

    [Display(Name = "文件类型名称")]
    public string ContentTypeName { get; set; }

    [Display(Name = "文件类型名称(EN)")]
    public string ContentTypeNameEn { get; set; }

    [Display(Name = "源名称")]
    public string OriginalName { get; set; }

    [Display(Name = "新名称")]
    public string NewName { get; set; }

    [Display(Name = "存储路径")]
    public string FilePath { get; set; }

    [Display(Name = "文件大小")]
    public string Size { get; set; }
}
