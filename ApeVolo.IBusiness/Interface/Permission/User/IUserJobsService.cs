using System.Collections.Generic;
using System.Threading.Tasks;
using ApeVolo.Entity.Do.Core;
using ApeVolo.IBusiness.Dto.Permission.User;

namespace ApeVolo.IBusiness.Interface.Permission.User;

/// <summary>
/// 用户岗位接口
/// </summary>
public interface IUserJobsService
{
    #region 基础接口

    Task<bool> CreateAsync(List<CreateUpdateUserJobsDto> createUpdateUserJobsDtos);
    Task<bool> DeleteByUserIdAsync(long userId);
    Task<List<UserJobs>> QueryByUserIdAsync(long userId);

    #endregion
}