using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ApeVolo.Common.AttributeExt;
using ApeVolo.IBusiness.Base;

namespace ApeVolo.IBusiness.Dto.Permission.Menu;

[AutoMapping(typeof(Entity.Do.Core.Menu), typeof(MenuDto))]
public class MenuDto : EntityDtoRoot<long>
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
    public bool IFrame { get; set; }

    [Display(Name = "Menu.Component")]
    public string Component { get; set; }

    [Display(Name = "Menu.ComponentName")]
    public string ComponentName { get; set; }

    [Display(Name = "Menu.PId")]
    public long? PId { get; set; }

    [Display(Name = "Menu.MenuSort")]
    public int MenuSort { get; set; }

    [Display(Name = "Menu.Icon")]
    public string Icon { get; set; }

    [Display(Name = "Menu.Type")]
    public int Type { get; set; }

    [Display(Name = "Menu.Cache")]
    public bool Cache { get; set; }

    [Display(Name = "Menu.Hidden")]
    public bool Hidden { get; set; }

    [Display(Name = "Menu.SubCount")]
    public int SubCount { get; set; }

    public List<MenuDto> Children { get; set; }

    public bool Leaf => SubCount == 0;
    public bool HasChildren => SubCount > 0;

    public string Label
    {
        get { return Title; }
    }
}