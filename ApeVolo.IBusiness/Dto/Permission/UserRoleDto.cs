using System.ComponentModel.DataAnnotations;
using ApeVolo.Common.AttributeExt;
using ApeVolo.Entity.Permission;
using ApeVolo.IBusiness.Base;

namespace ApeVolo.IBusiness.Dto.Permission;

[AutoMapping(typeof(Role), typeof(UserRoleDto))]
public class UserRoleDto
{
    [RegularExpression(@"^\+?[1-9]\d*$", ErrorMessage = "{0}required")]
    public long Id { get; set; }

    [Display(Name = "Role.Name")]
    [Required(ErrorMessage = "{0}required")]
    public string Name { get; set; }
}
