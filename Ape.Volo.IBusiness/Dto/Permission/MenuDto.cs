using System.Collections.Generic;
using Ape.Volo.Common.AttributeExt;
using Ape.Volo.Entity.Permission;
using Ape.Volo.IBusiness.Base;

namespace Ape.Volo.IBusiness.Dto.Permission;

[AutoMapping(typeof(Menu), typeof(MenuDto))]
public class MenuDto : BaseEntityDto<long>
{
    public string Title { get; set; }

    public string LinkUrl { get; set; }

    public string Path { get; set; }

    public string Permission { get; set; }

    public bool IFrame { get; set; }

    public string Component { get; set; }

    public string ComponentName { get; set; }

    public long ParentId { get; set; }

    public int Sort { get; set; }

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
