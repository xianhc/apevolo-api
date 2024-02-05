using System.Collections.Generic;
using System.Threading.Tasks;
using Ape.Volo.Common.Model;
using Ape.Volo.Entity.System;
using Ape.Volo.IBusiness.Base;
using Ape.Volo.IBusiness.Dto.System;
using Ape.Volo.IBusiness.QueryModel;

namespace Ape.Volo.IBusiness.Interface.System;

/// <summary>
/// QuartzJob作业接口
/// </summary>
public interface IQuartzNetService : IBaseServices<QuartzNet>
{
    #region 基础接口

    /// <summary>
    /// 查询全部
    /// </summary>
    /// <returns></returns>
    Task<List<QuartzNet>> QueryAllAsync();

    /// <summary>
    /// 创建
    /// </summary>
    /// <param name="createUpdateQuartzNetDto"></param>
    /// <returns></returns>
    Task<QuartzNet> CreateAsync(CreateUpdateQuartzNetDto createUpdateQuartzNetDto);

    /// <summary>
    /// 更新
    /// </summary>
    /// <param name="createUpdateQuartzNetDto"></param>
    /// <returns></returns>
    Task<bool> UpdateAsync(CreateUpdateQuartzNetDto createUpdateQuartzNetDto);

    /// <summary>
    /// 更新任务与日志
    /// </summary>
    /// <param name="quartzNet"></param>
    /// <param name="quartzNetLog"></param>
    /// <returns></returns>
    Task<bool> UpdateJobInfoAsync(QuartzNet quartzNet, QuartzNetLog quartzNetLog);

    /// <summary>
    /// 删除
    /// </summary>
    /// <param name="quartzNets"></param>
    /// <returns></returns>
    Task<bool> DeleteAsync(List<QuartzNet> quartzNets);

    /// <summary>
    /// 查询
    /// </summary>
    /// <param name="quartzNetQueryCriteria"></param>
    /// <param name="pagination"></param>
    /// <returns></returns>
    Task<List<QuartzNetDto>> QueryAsync(QuartzNetQueryCriteria quartzNetQueryCriteria, Pagination pagination);

    /// <summary>
    /// 下载
    /// </summary>
    /// <param name="quartzNetQueryCriteria"></param>
    /// <returns></returns>
    Task<List<ExportBase>> DownloadAsync(QuartzNetQueryCriteria quartzNetQueryCriteria);

    #endregion
}
