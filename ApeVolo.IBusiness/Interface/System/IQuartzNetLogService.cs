using System.Collections.Generic;
using System.Threading.Tasks;
using ApeVolo.Common.Model;
using ApeVolo.Entity.System;
using ApeVolo.IBusiness.Base;
using ApeVolo.IBusiness.Dto.System;
using ApeVolo.IBusiness.QueryModel;

namespace ApeVolo.IBusiness.Interface.System;

/// <summary>
/// QuartzJob日志接口
/// </summary>
public interface IQuartzNetLogService : IBaseServices<QuartzNetLog>
{
    #region 基础接口

    Task<bool> CreateAsync(QuartzNetLog quartzNetLog);
    Task<List<QuartzNetLogDto>> QueryAsync(QuartzNetLogQueryCriteria quartzNetLogQueryCriteria, Pagination pagination);
    Task<List<ExportBase>> DownloadAsync(QuartzNetLogQueryCriteria quartzNetLogQueryCriteria);

    #endregion
}
