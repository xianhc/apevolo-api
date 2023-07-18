using ApeVolo.Common.AttributeExt;
using ApeVolo.Entity.Permission;
using ApeVolo.IBusiness.Base;

namespace ApeVolo.IBusiness.Dto.Permission;

[AutoMapping(typeof(UserRoles), typeof(CreateUpdateUserRolesDto))]
public class CreateUpdateUserRolesDto : BaseEntityDto<long>
{
    public CreateUpdateUserRolesDto(long userId, long roleId)
    {
        UserId = userId;
        RoleId = roleId;
    }

    public long UserId { get; set; }

    public long RoleId { get; set; }
}
