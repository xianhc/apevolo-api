using System.Collections.Generic;
using System.Threading.Tasks;
using ApeVolo.Business.Base;
using ApeVolo.Common.WebApp;
using ApeVolo.Entity.Permission.Role;
using ApeVolo.IBusiness.Interface.Permission.Role;
using ApeVolo.IRepository.Permission.Role;

namespace ApeVolo.Business.Permission.Role;

/// <summary>
/// 角色菜单服务
/// </summary>
public class RolesMenusService : BaseServices<RoleMenu>, IRolesMenusService
{
    #region 构造函数

    public RolesMenusService(IRolesMenusRepository rolesMenusRepository, ICurrentUser currentUser)
    {
        BaseDal = rolesMenusRepository;
        CurrentUser = currentUser;
    }

    #endregion

    #region 基础方法

    public async Task<bool> DeleteAsync(List<long> roleIds)
    {
        var roleMenus = await BaseDal.QueryListAsync(x => roleIds.Contains(x.RoleId));
        return await DeleteEntityListAsync(roleMenus);
    }

    public async Task<bool> CreateAsync(List<RoleMenu> roleMenu)
    {
        return await AddEntityListAsync(roleMenu);
    }

    #endregion
}