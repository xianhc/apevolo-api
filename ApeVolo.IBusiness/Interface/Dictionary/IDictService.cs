using ApeVolo.Common.Model;
using ApeVolo.IBusiness.Dto.Dictionary;
using ApeVolo.IBusiness.QueryModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApeVolo.IBusiness.Base;
using ApeVolo.IBusiness.EditDto.Dict;
using ApeVolo.Common.Helper.Excel;
using ApeVolo.Entity.Do.Dictionary;

namespace ApeVolo.IBusiness.Interface.Dictionary
{
    /// <summary>
    /// 字典接口
    /// </summary>
    public interface IDictService : IBaseServices<Dict>
    {
        #region 基础接口

        Task<bool> CreateAsync(CreateUpdateDictDto createUpdateDictDto);

        Task<bool> UpdateAsync(CreateUpdateDictDto createUpdateDictDto);

        Task<bool> DeleteAsync(HashSet<string> ids);

        Task<List<DictDto>> QueryAsync(DictQueryCriteria dictQueryCriteria, Pagination pagination);

        Task<List<ExportRowModel>> DownloadAsync(DictQueryCriteria dictQueryCriteria);

        #endregion
    }
}