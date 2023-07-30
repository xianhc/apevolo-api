using System.ComponentModel.DataAnnotations;
using ApeVolo.Common.Global;
using ApeVolo.Common.Model;

namespace ApeVolo.IBusiness.ExportModel.Permission;

public class MenuExport : ExportBase
{
    [Display(Name = "菜单标题")]
    public string Title { get; set; }


    [Display(Name = "菜单URL")]
    public string LinkUrl { get; set; }

    [Display(Name = "组件路径")]
    public string Path { get; set; }

    [Display(Name = "权限标识符")]
    public string Permission { get; set; }

    [Display(Name = "是否IFrame")]
    public BoolState IsFrame { get; set; }

    [Display(Name = "组件")]
    public string Component { get; set; }

    [Display(Name = "组件名称")]
    public string ComponentName { get; set; }

    [Display(Name = "菜单父ID")]
    public long PId { get; set; }

    [Display(Name = "排序")]
    public int Sort { get; set; }

    [Display(Name = "Icon图标")]
    public string Icon { get; set; }

    [Display(Name = "菜单类型")]
    public string MenuType { get; set; }

    [Display(Name = "是否缓存")]
    public BoolState IsCache { get; set; }

    [Display(Name = "是否隐藏")]
    public BoolState IsHidden { get; set; }

    [Display(Name = "子菜单个数")]
    public int SubCount { get; set; }
}
