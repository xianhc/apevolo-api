using System.Collections.Generic;
using System.Threading.Tasks;
using Ape.Volo.Common.Model;
using Ape.Volo.IBusiness.Base;
using Ape.Volo.IBusiness.Dto.Permission;
using Ape.Volo.IBusiness.QueryModel;
using ApeVolo.Entity.Permission;

namespace Ape.Volo.IBusiness.Interface.Permission;

/// <summary>
/// 岗位接口
/// </summary>
public interface IJobService : IBaseServices<Job>
{
    #region 基础接口

    Task<bool> CreateAsync(CreateUpdateJobDto createUpdateJobDto);
    Task<bool> UpdateAsync(CreateUpdateJobDto createUpdateJobDto);
    Task<bool> DeleteAsync(HashSet<long> ids);
    Task<List<JobDto>> QueryAsync(JobQueryCriteria jobQueryCriteria, Pagination pagination);
    Task<List<ExportBase>> DownloadAsync(JobQueryCriteria jobQueryCriteria);

    #endregion

    #region 扩展接口

    /// <summary>
    /// 获取所有岗位
    /// </summary>
    /// <returns></returns>
    Task<List<JobDto>> QueryAllAsync();

    #endregion
}
