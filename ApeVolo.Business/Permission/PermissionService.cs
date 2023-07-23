using System.Collections.Generic;
using System.Threading.Tasks;
using ApeVolo.Business.Base;
using ApeVolo.Common.AttributeExt;
using ApeVolo.Common.Global;
using ApeVolo.Entity.Permission;
using ApeVolo.IBusiness.Interface.Permission;
using ApeVolo.IBusiness.Vo;
using SqlSugar;

namespace ApeVolo.Business.Permission;

public class PermissionService : BaseServices<Role>, IPermissionService
{
    #region 基础方法

    /// <summary>
    /// 获取用户权限
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    [UseCache(Expiration = 30, KeyPrefix = GlobalConstants.CacheKey.UserPermissionById)]
    public async Task<List<PermissionVo>> QueryUserPermissionAsync(long userId)
    {
        var permissionLists =
            await SugarRepository.QueryMuchAsync<Menu, RoleMenu, UserRoles, PermissionVo>(
                (m, rm, ur) => new object[]
                {
                    JoinType.Left, m.Id == rm.MenuId,
                    JoinType.Left, rm.RoleId == ur.RoleId
                },
                (m, rm, ur) => new PermissionVo
                {
                    LinkUrl = m.LinkUrl,
                    Permission = m.Permission
                },
                (m, rm, ur) => ur.UserId == userId
                , (m, rm, ur) => new { m.LinkUrl, m.Permission }
            );
        return permissionLists;
    }

    #endregion
}
