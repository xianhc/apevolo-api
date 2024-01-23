using System.ComponentModel.DataAnnotations;
using Ape.Volo.Common.AttributeExt;
using Ape.Volo.Entity.Permission;
using Ape.Volo.IBusiness.Base;

namespace Ape.Volo.IBusiness.Dto.Permission;

[AutoMapping(typeof(Department), typeof(CreateUpdateDepartmentDto))]
public class CreateUpdateDepartmentDto : BaseEntityDto<long>
{
    [Required]
    public string Name { get; set; }

    public long? ParentId { get; set; }

    [Range(1, 999)]
    public int Sort { get; set; }

    public bool Enabled { get; set; }
}
