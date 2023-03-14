using System.ComponentModel.DataAnnotations;
using ApeVolo.Common.AttributeExt;
using ApeVolo.IBusiness.Base;

namespace ApeVolo.IBusiness.Dto.Permission.User;

[AutoMapping(typeof(Entity.Do.Core.Role), typeof(UserRoleDto))]
public class UserRoleDto : RootId<long>
{
    [Display(Name = "Role.Name")]
    [Required(ErrorMessage = "{0}required")]
    public string Name { get; set; }
}