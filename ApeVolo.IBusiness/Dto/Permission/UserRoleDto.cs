using System.ComponentModel.DataAnnotations;
using ApeVolo.Common.AttributeExt;
using ApeVolo.Entity.Permission;

namespace ApeVolo.IBusiness.Dto.Permission;

[AutoMapping(typeof(Role), typeof(UserRoleDto))]
public class UserRoleDto
{
    [RegularExpression(@"^\+?[1-9]\d*$")]
    public long Id { get; set; }

    [Required]
    public string Name { get; set; }
}
