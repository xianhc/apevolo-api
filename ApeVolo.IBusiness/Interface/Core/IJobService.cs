using ApeVolo.Common.Helper.Excel;
using ApeVolo.Common.Model;
using ApeVolo.IBusiness.Base;
using ApeVolo.IBusiness.Dto.Core;
using ApeVolo.IBusiness.EditDto.Core;
using ApeVolo.IBusiness.QueryModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApeVolo.Entity.Do.Core;

namespace ApeVolo.IBusiness.Interface.Core
{
    /// <summary>
    /// 岗位接口
    /// </summary>
    public interface IJobService : IBaseServices<Job>
    {
        #region 基础接口

        Task<bool> CreateAsync(CreateUpdateJobDto createUpdateJobDto);
        Task<bool> UpdateAsync(CreateUpdateJobDto createUpdateJobDto);
        Task<bool> DeleteAsync(HashSet<string> ids);
        Task<List<JobDto>> QueryAsync(JobQueryCriteria jobQueryCriteria, Pagination pagination);
        Task<List<ExportRowModel>> DownloadAsync(JobQueryCriteria jobQueryCriteria);

        #endregion

        #region 扩展接口

        /// <summary>
        /// 获取所有岗位
        /// </summary>
        /// <returns></returns>
        Task<List<JobDto>> QueryAllAsync();

        #endregion
    }
}