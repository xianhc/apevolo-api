using System.Collections.Generic;
using System.Threading.Tasks;
using Ape.Volo.Common.Model;
using Ape.Volo.Entity.System;
using Ape.Volo.IBusiness.Base;
using Ape.Volo.IBusiness.Dto.System;
using Ape.Volo.IBusiness.QueryModel;

namespace Ape.Volo.IBusiness.Interface.System;

/// <summary>
/// 字典详情接口
/// </summary>
public interface IDictDetailService : IBaseServices<DictDetail>
{
    #region 基础接口

    Task<bool> CreateAsync(CreateUpdateDictDetailDto createUpdateDictDetailDto);

    Task<bool> UpdateAsync(CreateUpdateDictDetailDto createUpdateDictDetailDto);

    Task<bool> DeleteAsync(string id);

    Task<List<DictDetailDto>> QueryAsync(DictDetailQueryCriteria dictDetailQueryCriteria,
        Pagination pagination);

    #endregion
}
