using System.Collections.Generic;
using System.Threading.Tasks;
using ApeVolo.Entity.Do.Core;
using ApeVolo.IBusiness.Base;

namespace ApeVolo.IBusiness.Interface.Permission.Role;

/// <summary>
/// 角色菜单
/// </summary>
public interface IRolesMenusService : IBaseServices<RoleMenu>
{
    #region 角色接口

    Task<bool> CreateAsync(List<RoleMenu> roleMenu);

    Task<bool> DeleteAsync(List<long> roleIds);

    #endregion
}