using System.Collections.Generic;
using System.Threading.Tasks;
using ApeVolo.Entity.Permission;
using ApeVolo.IBusiness.Base;

namespace ApeVolo.IBusiness.Interface.Permission;

/// <summary>
/// 用户角色接口
/// </summary>
public interface IUserRolesService : IBaseServices<UserRoles>
{
    #region 基础接口

    Task<List<UserRoles>> QueryAsync(long userId);
    Task<List<UserRoles>> QueryByRoleIdsAsync(HashSet<long> roleIds);

    #endregion
}
