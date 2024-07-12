using Ape.Volo.Common.Attributes;
using Ape.Volo.Entity.Permission;
using Ape.Volo.IBusiness.Base;

namespace Ape.Volo.IBusiness.Dto.Permission;

/// <summary>
/// 用户角色Dto
/// </summary>
[AutoMapping(typeof(UserRole), typeof(CreateUpdateUserRoleDto))]
public class CreateUpdateUserRoleDto : BaseEntityDto<long>
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="roleId"></param>
    public CreateUpdateUserRoleDto(long userId, long roleId)
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
