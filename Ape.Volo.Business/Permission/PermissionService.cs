using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ape.Volo.Business.Base;
using Ape.Volo.Common.Attributes;
using Ape.Volo.Common.Enums;
using Ape.Volo.Common.Extensions;
using Ape.Volo.Common.Global;
using Ape.Volo.Common.Model;
using Ape.Volo.Entity.Permission;
using Ape.Volo.IBusiness.Interface.Permission;
using Ape.Volo.IBusiness.Vo;

namespace Ape.Volo.Business.Permission;

public class PermissionService : BaseServices<Role>, IPermissionService
{
    #region 基础方法

    /// <summary>
    /// 获取权限标识符
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    [UseCache(Expiration = 60, KeyPrefix = GlobalConstants.CachePrefix.UserPermissionRoles)]
    public async Task<List<string>> GetPermissionRolesAsync(long userId)
    {
        var permissionRoles = await SugarClient
            .Queryable<UserRole, RoleMenu, Menu>((ur, rm, m) => ur.RoleId == rm.RoleId && rm.MenuId == m.Id)
            .GroupBy((ur, rm, m) => m.Permission)
            .Where((ur, rm, m) => ur.UserId == userId && m.Type != MenuType.Catalog && m.Permission != null)
            .OrderBy((ur, rm, m) => m.Permission)
            .ClearFilter<ICreateByEntity>()
            .Select((ur, rm, m) => m.Permission).ToListAsync();
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
        var permissionVos = await SugarClient
            .Queryable<UserRole, RoleApis, Apis>((ur, ra, a) => ur.RoleId == ra.RoleId && ra.ApisId == a.Id)
            .GroupBy((ur, ra, a) => new { a.Url, a.Method })
            .Where(ur => ur.UserId == userId)
            .OrderBy((ur, ra, a) => a.Url)
            .ClearFilter<ICreateByEntity>()
            .Select((ur, ra, a) => new PermissionVo()
            {
                Url = a.Url,
                Method = a.Method
            }).ToListAsync();
        permissionVos = permissionVos.Where(x => !x.IsNullOrEmpty()).ToList();
        return permissionVos;
    }

    #endregion
}
