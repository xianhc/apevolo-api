using System.Collections.Generic;
using System.Threading.Tasks;
using Ape.Volo.Common.Model;
using Ape.Volo.IBusiness.Base;
using Ape.Volo.IBusiness.Dto.System;
using Ape.Volo.IBusiness.QueryModel;
using ApeVolo.Entity.System;

namespace Ape.Volo.IBusiness.Interface.System;

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

    Task<List<ExportBase>> DownloadAsync(DictQueryCriteria dictQueryCriteria);

    #endregion
}
