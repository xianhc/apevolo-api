using System.ComponentModel.DataAnnotations;
using Ape.Volo.Common.AttributeExt;
using Ape.Volo.Entity.Permission;
using Ape.Volo.IBusiness.Base;

namespace Ape.Volo.IBusiness.Dto.Permission;

[AutoMapping(typeof(Menu), typeof(CreateUpdateMenuDto))]
public class CreateUpdateMenuDto : BaseEntityDto<long>
{
    [Required]
    public string Title { get; set; }

    public string LinkUrl { get; set; }

    public string Path { get; set; }

    public string Permission { get; set; }

    public bool IFrame { get; set; }

    public string Component { get; set; }

    public string ComponentName { get; set; }

    public long ParentId { get; set; }

    [Range(1, 999)]
    public int Sort { get; set; }

    public string Icon { get; set; }

    [Range(1, 3)]
    public int Type { get; set; }

    public bool Cache { get; set; }

    public bool Hidden { get; set; }

    public int SubCount { get; set; }
}
