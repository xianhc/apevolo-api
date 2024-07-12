using System.Collections.Generic;
using System.Threading.Tasks;
using Ape.Volo.Entity.System;
using Ape.Volo.IBusiness.Base;
using Ape.Volo.IBusiness.Dto.System;

namespace Ape.Volo.IBusiness.Interface.System;

/// <summary>
/// 字典详情接口
/// </summary>
public interface IDictDetailService : IBaseServices<DictDetail>
{
    #region 基础接口

    /// <summary>
    /// 创建
    /// </summary>
    /// <param name="createUpdateDictDetailDto"></param>
    /// <returns></returns>
    Task<bool> CreateAsync(CreateUpdateDictDetailDto createUpdateDictDetailDto);

    /// <summary>
    /// 更新
    /// </summary>
    /// <param name="createUpdateDictDetailDto"></param>
    /// <returns></returns>
    Task<bool> UpdateAsync(CreateUpdateDictDetailDto createUpdateDictDetailDto);

    /// <summary>
    /// 删除
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<bool> DeleteAsync(long id);

    /// <summary>
    /// 查询
    /// </summary>
    /// <param name="dictName"></param>
    /// <returns></returns>
    Task<List<DictDetailDto>> QueryAsync(string dictName);

    #endregion
}
