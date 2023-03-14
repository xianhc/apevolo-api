using System.Collections.Generic;
using System.Threading.Tasks;
using ApeVolo.Entity.Do.Core;
using ApeVolo.IBusiness.Base;
using ApeVolo.IBusiness.Dto.Permission.User;

namespace ApeVolo.IBusiness.Interface.Permission.User;

/// <summary>
/// 用户角色接口
/// </summary>
public interface IUserRolesService : IBaseServices<UserRoles>
{
    #region 基础接口

    Task<bool> CreateAsync(List<CreateUpdateUserRolesDto> createUpdateUserRoleDtos);
    Task<bool> DeleteByUserIdAsync(long userId);
    Task<List<UserRoles>> QueryAsync(long userId);
    Task<List<UserRoles>> QueryByRoleIdsAsync(HashSet<long> roleIds);

    #endregion
}