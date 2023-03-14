using System.Collections.Generic;
using System.Threading.Tasks;
using ApeVolo.Common.Model;
using ApeVolo.Entity.Do.Dictionary;
using ApeVolo.IBusiness.Base;
using ApeVolo.IBusiness.Dto.System.Dict.Detail;
using ApeVolo.IBusiness.QueryModel;

namespace ApeVolo.IBusiness.Interface.System.Dictionary;

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