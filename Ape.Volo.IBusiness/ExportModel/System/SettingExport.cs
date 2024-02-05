using System.ComponentModel.DataAnnotations;
using Ape.Volo.Common.Global;
using Ape.Volo.Common.Model;

namespace Ape.Volo.IBusiness.ExportModel.System;

/// <summary>
/// 设置导出模板
/// </summary>
public class SettingExport : ExportBase
{
    /// <summary>
    /// 键
    /// </summary>
    [Display(Name = "键")]
    public string Name { get; set; }

    /// <summary>
    /// 值
    /// </summary>
    [Display(Name = "值")]
    public string Value { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    [Display(Name = "是否启用")]
    public EnabledState EnabledState { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    [Display(Name = "描述")]
    public string Description { get; set; }
}
