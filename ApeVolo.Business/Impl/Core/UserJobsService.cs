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

namespace ApeVolo.Business.Impl.Core
{
    /// <summary>
    /// 用户与岗位服务
    /// </summary>
    public class UserJobsService : BaseServices<UserJobs>, IUserJobsService
    {
        #region 构造函数

        public UserJobsService(IUserJobsRepository userJobsRepository, IMapper mapper)
        {
            _baseDal = userJobsRepository;
            _mapper = mapper;
        }

        #endregion

        #region 基础方法

        public async Task<int> CreateAsync(List<CreateUpdateUserJobsDto> createUpdateJobDtos)
        {
            var userJobs = _mapper.Map<List<UserJobs>>(createUpdateJobDtos);
            return await _baseDal.AddAsync(userJobs);
        }

        public async Task<bool> DeleteByUserIdAsync(long userId)
        {
            if (userId.IsNullOrEmpty())
            {
                throw new BadRequestException("userId 不能为空！");
            }

            return await _baseDal.DeleteAsync(x => x.UserId == userId) > 0;
        }

        [RedisCaching(KeyPrefix = RedisKey.UserJobsById)]
        public async Task<List<UserJobs>> QueryByUserIdAsync(long userId)
        {
            return await _baseDal.QueryListAsync(uj => uj.UserId == userId);
        }

        #endregion
    }
}