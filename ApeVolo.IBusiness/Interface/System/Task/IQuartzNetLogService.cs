using System.Collections.Generic;
using System.Threading.Tasks;
using ApeVolo.Common.Helper.Excel;
using ApeVolo.Common.Model;
using ApeVolo.Entity.Do.Tasks;
using ApeVolo.IBusiness.Base;
using ApeVolo.IBusiness.Dto.System.Task;
using ApeVolo.IBusiness.QueryModel;

namespace ApeVolo.IBusiness.Interface.System.Task;

/// <summary>
/// QuartzJob日志接口
/// </summary>
public interface IQuartzNetLogService : IBaseServices<QuartzNetLog>
{
    #region 基础接口

    Task<bool> CreateAsync(QuartzNetLog quartzNetLog);
    Task<List<QuartzNetLogDto>> QueryAsync(QuartzNetLogQueryCriteria quartzNetLogQueryCriteria, Pagination pagination);
    Task<List<ExportRowModel>> DownloadAsync(QuartzNetLogQueryCriteria quartzNetLogQueryCriteria);

    #endregion
}