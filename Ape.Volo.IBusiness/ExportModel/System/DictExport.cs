using System.ComponentModel.DataAnnotations;
using Ape.Volo.Common.Enums;
using Ape.Volo.Common.Model;

namespace Ape.Volo.IBusiness.ExportModel.System;

/// <summary>
/// 字典导出模板
/// </summary>
public class DictExport : ExportBase
{
    /// <summary>
    /// 字典类型
    /// </summary>
    [Display(Name = "字典类型")]
    public DictType DictType { get; set; }

    /// <summary>
    /// 字典名称
    /// </summary>
    [Display(Name = "字典名称")]
    public string Name { get; set; }

    /// <summary>
    /// 字典描述
    /// </summary>
    [Display(Name = "字典描述")]
    public string Description { get; set; }

    /// <summary>
    /// 字典标签
    /// </summary>
    [Display(Name = "字典标签")]
    public string Lable { get; set; }

    /// <summary>
    /// 字典值
    /// </summary>
    [Display(Name = "字典值")]
    public string Value { get; set; }
}
