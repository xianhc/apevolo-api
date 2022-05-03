using System.Collections.Generic;
using ApeVolo.Common.AttributeExt;
using ApeVolo.Entity.Do.Core;

namespace ApeVolo.IBusiness.Dto.Core;

[AutoMapping(typeof(Menu), typeof(MenuDto))]
public class MenuDto : EntityDtoRoot<long>
{
    public string Title { get; set; }

    //[JsonIgnore] 
    public string LinkUrl { get; set; }

    public string Path { get; set; }

    public string Permission { get; set; }

    public bool IFrame { get; set; }

    public string Component { get; set; }

    public string ComponentName { get; set; }

    public long? PId { get; set; }

    public int MenuSort { get; set; }

    public string Icon { get; set; }

    public int Type { get; set; }

    public bool Cache { get; set; }
    public bool Hidden { get; set; }

    public int SubCount { get; set; }

    public List<MenuDto> Children { get; set; }

    public bool Leaf => SubCount == 0;
    public bool HasChildren => SubCount > 0;

    public string Label
    {
        get { return Title; }
    }
}