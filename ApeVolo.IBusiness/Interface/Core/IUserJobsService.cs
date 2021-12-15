using System.Collections.Generic;
using System.Threading.Tasks;
using ApeVolo.Entity.Do.Core;
using ApeVolo.IBusiness.EditDto.Core;

namespace ApeVolo.IBusiness.Interface.Core
{
    /// <summary>
    /// 用户岗位接口
    /// </summary>
    public interface IUserJobsService
    {
        #region 基础接口
        Task<int> CreateAsync(List<CreateUpdateUserJobsDto> createUpdateUserJobsDtos);
        Task<bool> DeleteByUserIdAsync(long userId);
        Task<List<UserJobs>> QueryByUserIdAsync(long userId);

        #endregion
    }
}