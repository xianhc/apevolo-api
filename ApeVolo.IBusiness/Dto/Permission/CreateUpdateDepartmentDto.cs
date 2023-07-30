using System.ComponentModel.DataAnnotations;
using ApeVolo.Common.AttributeExt;
using ApeVolo.IBusiness.Base;

namespace ApeVolo.IBusiness.Dto.Permission;

[AutoMapping(typeof(Entity.Permission.Department), typeof(CreateUpdateDepartmentDto))]
public class CreateUpdateDepartmentDto : BaseEntityDto<long>
{
    [Required]
    public string Name { get; set; }

    public long? ParentId { get; set; }
    
    [Range(1, 999)]
    public int Sort { get; set; }

    public bool Enabled { get; set; }
}
