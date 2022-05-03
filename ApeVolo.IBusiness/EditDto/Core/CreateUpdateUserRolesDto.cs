using ApeVolo.Common.AttributeExt;
using ApeVolo.Entity.Do.Core;
using ApeVolo.IBusiness.Dto;

namespace ApeVolo.IBusiness.EditDto.Core;

[AutoMapping(typeof(UserRoles), typeof(CreateUpdateUserRolesDto))]
public class CreateUpdateUserRolesDto : EntityDtoRoot<long>
{
    public CreateUpdateUserRolesDto(long userId, long roleId)
    {
        UserId = userId;
        RoleId = roleId;
    }

    public long UserId { get; set; }

    public long RoleId { get; set; }
}