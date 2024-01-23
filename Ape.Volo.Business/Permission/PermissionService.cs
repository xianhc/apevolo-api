using System.Collections.Generic;
using System.Threading.Tasks;
using Ape.Volo.Business.Base;
using Ape.Volo.Common.AttributeExt;
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
