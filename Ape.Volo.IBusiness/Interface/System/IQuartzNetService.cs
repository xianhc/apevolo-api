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

    Task<List<QuartzNet>> QueryAllAsync();

    Task<QuartzNet> CreateAsync(CreateUpdateQuartzNetDto createUpdateQuartzNetDto);

    Task<bool> UpdateAsync(CreateUpdateQuartzNetDto createUpdateQuartzNetDto);

    Task<bool> UpdateJobInfoAsync(QuartzNet quartzNet, QuartzNetLog quartzNetLog);

    Task<bool> DeleteAsync(List<QuartzNet> quartzNets);

    Task<List<QuartzNetDto>> QueryAsync(QuartzNetQueryCriteria quartzNetQueryCriteria, Pagination pagination);

    Task<List<ExportBase>> DownloadAsync(QuartzNetQueryCriteria quartzNetQueryCriteria);

    #endregion
}
