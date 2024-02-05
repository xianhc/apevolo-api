using System.ComponentModel.DataAnnotations;
using Ape.Volo.Common.Global;
using Ape.Volo.Common.Model;

namespace Ape.Volo.IBusiness.ExportModel.Permission;

/// <summary>
/// 岗位导出模板
/// </summary>
public class JobExport : ExportBase
{
    /// <summary>
    /// 岗位名称
    /// </summary>
    [Display(Name = "岗位名称")]
    public string Name { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    [Display(Name = "排序")]
    public int Sort { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    [Display(Name = "是否启用")]
    public EnabledState EnabledState { get; set; }
}
