using Ape.Volo.Common.AttributeExt;
using Ape.Volo.Entity.Permission;
using Ape.Volo.IBusiness.Base;

namespace Ape.Volo.IBusiness.Dto.Permission;

/// <summary>
/// 用户角色Dto
/// </summary>
[AutoMapping(typeof(UserRoles), typeof(CreateUpdateUserRolesDto))]
public class CreateUpdateUserRolesDto : BaseEntityDto<long>
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="roleId"></param>
    public CreateUpdateUserRolesDto(long userId, long roleId)
    {
        UserId = userId;
        RoleId = roleId;
    }

    /// <summary>
    /// 用户ID
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// 角色ID
    /// </summary>
    public long RoleId { get; set; }
}
