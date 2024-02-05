using System.Collections.Generic;
using System.Threading.Tasks;
using Ape.Volo.Common.Model;
using Ape.Volo.Entity.Permission;
using Ape.Volo.IBusiness.Base;
using Ape.Volo.IBusiness.Dto.Permission;
using Ape.Volo.IBusiness.QueryModel;

namespace Ape.Volo.IBusiness.Interface.Permission;

/// <summary>
/// 岗位接口
/// </summary>
public interface IJobService : IBaseServices<Job>
{
    #region 基础接口

    /// <summary>
    /// 创建
    /// </summary>
    /// <param name="createUpdateJobDto"></param>
    /// <returns></returns>
    Task<bool> CreateAsync(CreateUpdateJobDto createUpdateJobDto);

    /// <summary>
    /// 更新
    /// </summary>
    /// <param name="createUpdateJobDto"></param>
    /// <returns></returns>
    Task<bool> UpdateAsync(CreateUpdateJobDto createUpdateJobDto);

    /// <summary>
    /// 删除
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    Task<bool> DeleteAsync(HashSet<long> ids);

    /// <summary>
    /// 查询
    /// </summary>
    /// <param name="jobQueryCriteria"></param>
    /// <param name="pagination"></param>
    /// <returns></returns>
    Task<List<JobDto>> QueryAsync(JobQueryCriteria jobQueryCriteria, Pagination pagination);

    /// <summary>
    /// 下载
    /// </summary>
    /// <param name="jobQueryCriteria"></param>
    /// <returns></returns>
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
