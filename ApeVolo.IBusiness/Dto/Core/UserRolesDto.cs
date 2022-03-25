using ApeVolo.Common.AttributeExt;
using ApeVolo.Entity.Do.Core;

namespace ApeVolo.IBusiness.Dto.Core;

[AutoMapping(typeof(UserRoles), typeof(UserRolesDto))]
public class UserRolesDto
{
    public string UserId { get; set; }

    public string RoleId { get; set; }
}