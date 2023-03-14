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

    Task<List<ExportRowModel>> DownloadAsync(QuartzNetQueryCriteria quartzNetQueryCriteria);

    #endregion
}