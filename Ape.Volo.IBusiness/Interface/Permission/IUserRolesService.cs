using System.Collections.Generic;
using System.Threading.Tasks;
using Ape.Volo.Entity.Permission;
using Ape.Volo.IBusiness.Base;

namespace Ape.Volo.IBusiness.Interface.Permission;

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
