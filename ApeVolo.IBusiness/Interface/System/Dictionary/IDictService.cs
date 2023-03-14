using System.Collections.Generic;
using System.Threading.Tasks;
using ApeVolo.Common.Helper.Excel;
using ApeVolo.Common.Model;
using ApeVolo.Entity.Do.Dictionary;
using ApeVolo.IBusiness.Base;
using ApeVolo.IBusiness.Dto.System.Dict;
using ApeVolo.IBusiness.QueryModel;

namespace ApeVolo.IBusiness.Interface.System.Dictionary;

/// <summary>
/// 字典接口
/// </summary>
public interface IDictService : IBaseServices<Dict>
{
    #region 基础接口

    Task<bool> CreateAsync(CreateUpdateDictDto createUpdateDictDto);

    Task<bool> UpdateAsync(CreateUpdateDictDto createUpdateDictDto);

    Task<bool> DeleteAsync(HashSet<long> ids);

    Task<List<DictDto>> QueryAsync(DictQueryCriteria dictQueryCriteria, Pagination pagination);

    Task<List<ExportRowModel>> DownloadAsync(DictQueryCriteria dictQueryCriteria);

    #endregion
}