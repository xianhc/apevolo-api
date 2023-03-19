using System.Collections.Generic;
using System.Threading.Tasks;
using ApeVolo.Business.Base;
using ApeVolo.Common.AttributeExt;
using ApeVolo.Common.Global;
using ApeVolo.Entity.Permission.User;
using ApeVolo.IBusiness.Dto.Permission.User;
using ApeVolo.IBusiness.Interface.Permission.User;
using ApeVolo.IRepository.Permission.User;
using AutoMapper;

namespace ApeVolo.Business.Permission.User;

/// <summary>
/// 用户与岗位服务
/// </summary>
public class UserJobsService : BaseServices<UserJobs>, IUserJobsService
{
    #region 构造函数

    public UserJobsService(IUserJobsRepository userJobsRepository, IMapper mapper)
    {
        BaseDal = userJobsRepository;
        Mapper = mapper;
    }

    #endregion

    #region 基础方法

    public async Task<bool> CreateAsync(List<CreateUpdateUserJobsDto> createUpdateUserJobsDtos)
    {
        var userJobs = Mapper.Map<List<UserJobs>>(createUpdateUserJobsDtos);
        return await AddEntityListAsync(userJobs);
    }

    public async Task<bool> DeleteByUserIdAsync(long userId)
    {
        var userJobs = await BaseDal.QueryListAsync(x => x.UserId == userId);
        return await DeleteEntityListAsync(userJobs);
    }

    [RedisCaching(KeyPrefix = RedisKey.UserJobsById)]
    public async Task<List<UserJobs>> QueryByUserIdAsync(long userId)
    {
        return await BaseDal.QueryListAsync(uj => uj.UserId == userId);
    }

    #endregion
}