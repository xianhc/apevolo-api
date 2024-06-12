using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ape.Volo.Business.Base;
using Ape.Volo.Common.AttributeExt;
using Ape.Volo.Common.Extensions;
using Ape.Volo.Common.Global;
using Ape.Volo.Entity.Permission;
using Ape.Volo.IBusiness.Interface.Permission;
using Ape.Volo.IBusiness.Vo;
using SqlSugar;

namespace Ape.Volo.Business.Permission;

public class PermissionService : BaseServices<Role>, IPermissionService
{
    #region 基础方法

    /// <summary>
    /// 获取权限标识符
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    [UseCache(Expiration = 30, KeyPrefix = GlobalConstants.CachePrefix.UserPermissionRoles)]
    public async Task<List<string>> GetPermissionRolesAsync(long userId)
    {
        var permissionRoles =
            await SugarRepository.QueryMuchAsync<Menu, RoleMenu, UserRole, string>(
                (m, rm, ur) => new object[]
                {
                    JoinType.Left, m.Id == rm.MenuId,
                    JoinType.Left, rm.RoleId == ur.RoleId
                },
                (m, rm, ur) => m.Permission,
                (m, rm, ur) => ur.UserId == userId
            );
        permissionRoles = permissionRoles.Where(x => !x.IsNullOrEmpty()).ToList();
        return permissionRoles;
    }


    /// <summary>
    /// 获取权限urls
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    [UseCache(Expiration = 60, KeyPrefix = GlobalConstants.CachePrefix.UserPermissionUrls)]
    public async Task<List<PermissionVo>> GetPermissionVoAsync(long userId)
    {
        var permissionVos =
            await SugarRepository.QueryMuchAsync<Apis, RoleApis, UserRole, PermissionVo>(
                (m, rm, ur) => new object[]
                {
                    JoinType.Left, m.Id == rm.ApisId,
                    JoinType.Left, rm.RoleId == ur.RoleId
                },
                (m, rm, ur) => new PermissionVo()
                {
                    Url = m.Url,
                    Method = m.Method
                },
                (m, rm, ur) => ur.UserId == userId
            );
        permissionVos = permissionVos.Where(x => !x.IsNullOrEmpty()).ToList();
        return permissionVos;
    }

    #endregion
}
