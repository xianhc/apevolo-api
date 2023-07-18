using System.Collections.Generic;
using System.Threading.Tasks;
using ApeVolo.Business.Base;
using ApeVolo.Common.AttributeExt;
using ApeVolo.Common.Global;
using ApeVolo.Common.WebApp;
using ApeVolo.Entity.Permission;
using ApeVolo.IBusiness.Interface.Permission;
using SqlSugar;

namespace ApeVolo.Business.Permission;

/// <summary>
/// 用户与角色服务
/// </summary>
public class UserRolesService : BaseServices<UserRoles>, IUserRolesService
{
    #region 构造函数

    public UserRolesService(ApeContext apeContext) : base(apeContext)
    {
    }

    #endregion

    #region 基础方法

    [RedisCaching(KeyPrefix = GlobalConstants.CacheKey.UserRolesById)]
    public async Task<List<UserRoles>> QueryAsync(long userId)
    {
        return await SugarRepository.QueryListAsync(ur => ur.UserId == userId);
    }

    public async Task<List<UserRoles>> QueryByRoleIdsAsync(HashSet<long> roleIds)
    {
        var list = await SugarRepository.QueryMuchAsync<UserRoles, User, UserRoles>(
            (ur, u) => new object[]
            {
                JoinType.Left, ur.UserId == u.Id
            },
            (ur, u) => ur,
            (ur, u) => roleIds.Contains(ur.RoleId)
        );
        return list;
    }

    #endregion
}
