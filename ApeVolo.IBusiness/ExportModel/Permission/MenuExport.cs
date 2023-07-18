using System.ComponentModel.DataAnnotations;
using ApeVolo.Common.Global;
using ApeVolo.Common.Model;

namespace ApeVolo.IBusiness.ExportModel.Permission;

public class MenuExport : ExportBase
{
    [Display(Name = "Menu.Title")]
    public string Title { get; set; }


    [Display(Name = "Menu.LinkUrl")]
    public string LinkUrl { get; set; }

    [Display(Name = "Menu.Path")]
    public string Path { get; set; }

    [Display(Name = "Menu.Permission")]
    public string Permission { get; set; }

    [Display(Name = "Menu.IFrame")]
    public BoolState IsFrame { get; set; }

    [Display(Name = "Menu.Component")]
    public string Component { get; set; }

    [Display(Name = "Menu.ComponentName")]
    public string ComponentName { get; set; }

    [Display(Name = "Menu.PId")]
    public long PId { get; set; }

    [Display(Name = "Menu.MenuSort")]
    public int Sort { get; set; }

    [Display(Name = "Menu.Icon")]
    public string Icon { get; set; }

    [Display(Name = "Menu.Type")]
    public string MenuType { get; set; }

    [Display(Name = "Menu.Cache")]
    public BoolState IsCache { get; set; }

    [Display(Name = "Menu.Hidden")]
    public BoolState IsHidden { get; set; }

    [Display(Name = "Menu.SubCount")]
    public int SubCount { get; set; }
}
