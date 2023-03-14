using System.Collections.Generic;
using System.Threading.Tasks;
using ApeVolo.Business.Base;
using ApeVolo.Common.AttributeExt;
using ApeVolo.Common.Global;
using ApeVolo.Entity.Do.Core;
using ApeVolo.IBusiness.Interface.Permission.User;
using ApeVolo.IBusiness.Vo;
using ApeVolo.IRepository.Permission.Role;
using SqlSugar;

namespace ApeVolo.Business.Permission.User;

public class PermissionService : BaseServices<Entity.Do.Core.Role>, IPermissionService
{
    #region 字段

    #endregion

    #region 构造函数

    public PermissionService(IRoleRepository roleRepository)
    {
        BaseDal = roleRepository;
    }

    #endregion

    #region 基础方法

    /// <summary>
    /// 获取用户权限
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    [RedisCaching(Expiration = 30, KeyPrefix = RedisKey.UserPermissionById)]
    public async Task<List<PermissionVo>> QueryUserPermissionAsync(long userId)
    {
        var permissionLists = await BaseDal.QueryMuchAsync<Entity.Do.Core.Menu, RoleMenu, UserRoles, PermissionVo>(
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