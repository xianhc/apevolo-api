using System.ComponentModel.DataAnnotations;
using ApeVolo.Common.AttributeExt;
using ApeVolo.IBusiness.Base;

namespace ApeVolo.IBusiness.Dto.Permission;

[AutoMapping(typeof(Entity.Permission.Department), typeof(CreateUpdateDepartmentDto))]
public class CreateUpdateDepartmentDto : BaseEntityDto<long>
{
    [Display(Name = "Dept.Name")]
    [Required(ErrorMessage = "{0}required")]
    public string Name { get; set; }

    public long? ParentId { get; set; }

    [Display(Name = "Dept.Sort")]
    [Range(1, 999, ErrorMessage = "{0}range{1}{2}")]
    public int Sort { get; set; }

    public bool Enabled { get; set; }
}
