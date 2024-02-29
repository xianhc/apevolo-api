using System.ComponentModel.DataAnnotations;
using Ape.Volo.Common.Global;
using Ape.Volo.Common.Model;

namespace Ape.Volo.IBusiness.ExportModel.Permission;

/// <summary>
/// 菜单导出模板
/// </summary>
public class MenuExport : ExportBase
{
    /// <summary>
    /// 菜单标题
    /// </summary>
    [Display(Name = "菜单标题")]
    public string Title { get; set; }

    /// <summary>
    /// 组件路径
    /// </summary>
    [Display(Name = "组件路径")]
    public string Path { get; set; }

    /// <summary>
    /// 权限标识符
    /// </summary>
    [Display(Name = "权限标识符")]
    public string Permission { get; set; }

    /// <summary>
    /// 是否IFrame
    /// </summary>
    [Display(Name = "是否IFrame")]
    public BoolState IsFrame { get; set; }

    /// <summary>
    /// 组件
    /// </summary>
    [Display(Name = "组件")]
    public string Component { get; set; }

    /// <summary>
    /// 组件名称
    /// </summary>
    [Display(Name = "组件名称")]
    public string ComponentName { get; set; }

    /// <summary>
    /// 菜单父ID
    /// </summary>
    [Display(Name = "菜单父ID")]
    public long PId { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    [Display(Name = "排序")]
    public int Sort { get; set; }

    /// <summary>
    /// Icon图标
    /// </summary>
    [Display(Name = "Icon图标")]
    public string Icon { get; set; }

    /// <summary>
    /// 菜单类型
    /// </summary>
    [Display(Name = "菜单类型")]
    public string MenuType { get; set; }

    /// <summary>
    /// 是否缓存
    /// </summary>
    [Display(Name = "是否缓存")]
    public BoolState IsCache { get; set; }

    /// <summary>
    /// 是否隐藏
    /// </summary>
    [Display(Name = "是否隐藏")]
    public BoolState IsHidden { get; set; }

    /// <summary>
    /// 子菜单个数
    /// </summary>
    [Display(Name = "子菜单个数")]
    public int SubCount { get; set; }
}
