using System.Collections.Generic;
using System.Threading.Tasks;
using ApeVolo.Common.Model;
using ApeVolo.IBusiness.Base;
using ApeVolo.IBusiness.Dto.Permission;
using ApeVolo.IBusiness.QueryModel;

namespace ApeVolo.IBusiness.Interface.Permission;

/// <summary>
/// 岗位接口
/// </summary>
public interface IJobService : IBaseServices<Entity.Permission.Job>
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
