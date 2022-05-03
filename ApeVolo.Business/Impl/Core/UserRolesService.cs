using System.Collections.Generic;
using System.Threading.Tasks;
using ApeVolo.Business.Base;
using ApeVolo.Common.AttributeExt;
using ApeVolo.Common.Exception;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Global;
using ApeVolo.Entity.Do.Core;
using ApeVolo.IBusiness.EditDto.Core;
using ApeVolo.IBusiness.Interface.Core;
using ApeVolo.IRepository.Core;
using AutoMapper;
using SqlSugar;

namespace ApeVolo.Business.Impl.Core;

/// <summary>
/// 用户与角色服务
/// </summary>
public class UserRolesService : BaseServices<UserRoles>, IUserRolesService
{
    #region 构造函数

    public UserRolesService(IUserRolesRepository userRolesRepository, IMapper mapper)
    {
        BaseDal = userRolesRepository;
        Mapper = mapper;
    }

    #endregion

    #region 基础方法

    public async Task<bool> CreateAsync(List<CreateUpdateUserRolesDto> createUpdateUserRoleDtos)
    {
        var userRoles = Mapper.Map<List<UserRoles>>(createUpdateUserRoleDtos);
        return await AddEntityListAsync(userRoles);
    }

    public async Task<bool> DeleteByUserIdAsync(long userId)
    {
        var userRoles = await BaseDal.QueryListAsync(x => x.UserId == userId && x.IsDeleted == false);
        return await DeleteEntityListAsync(userRoles);
    }

    [RedisCaching(KeyPrefix = RedisKey.UserRolesById)]
    public async Task<List<UserRoles>> QueryAsync(long userId)
    {
        return await BaseDal.QueryListAsync(ur => ur.UserId == userId && ur.IsDeleted == false);
    }

    public async Task<List<UserRoles>> QueryByRoleIdsAsync(HashSet<long> roleIds)
    {
        var list = await BaseDal.QueryMuchAsync<UserRoles, User, UserRoles>(
            (ur, u) => new object[]
            {
                JoinType.Left, ur.UserId == u.Id
            },
            (ur, u) => ur,
            (ur, u) => u.IsDeleted == false && ur.IsDeleted == false && roleIds.Contains(ur.RoleId)
        );
        return list;
    }

    #endregion
}