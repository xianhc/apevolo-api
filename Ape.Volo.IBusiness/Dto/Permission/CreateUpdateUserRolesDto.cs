using Ape.Volo.Common.AttributeExt;
using Ape.Volo.Entity.Permission;
using Ape.Volo.IBusiness.Base;

namespace Ape.Volo.IBusiness.Dto.Permission;

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
