using System.Collections.Generic;
using System.Threading.Tasks;
using Ape.Volo.Business.Base;
using Ape.Volo.Common.AttributeExt;
using Ape.Volo.Common.Global;
using Ape.Volo.Common.WebApp;
using Ape.Volo.Entity.Permission;
using Ape.Volo.IBusiness.Interface.Permission;
using SqlSugar;

namespace Ape.Volo.Business.Permission;

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

    [UseCache(KeyPrefix = GlobalConstants.CacheKey.UserRolesById)]
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
