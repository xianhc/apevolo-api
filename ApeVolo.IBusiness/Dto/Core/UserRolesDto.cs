using ApeVolo.Common.AttributeExt;
using ApeVolo.Entity.Do.Core;

namespace ApeVolo.IBusiness.Dto.Core;

[AutoMapping(typeof(UserRoles), typeof(UserRolesDto))]
public class UserRolesDto : EntityDtoRoot<long>
{
    public long UserId { get; set; }

    public long RoleId { get; set; }
}