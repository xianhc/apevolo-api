using ApeVolo.IBusiness.EditDto.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApeVolo.Entity.Do.Core;

namespace ApeVolo.IBusiness.Interface.Core
{
    /// <summary>
    /// 用户岗位接口
    /// </summary>
    public interface IUserJobsService
    {
        #region 基础接口
        Task<int> CreateAsync(List<CreateUpdateUserJobsDto> createUpdateUserJobsDtos);
        Task<bool> DeleteByUserIdAsync(string userId);
        Task<List<UserJobs>> QueryByUserIdAsync(string userId);

        #endregion
    }
}