using System.Collections.Generic;
using System.Threading.Tasks;
using Ape.Volo.Common.Model;
using Ape.Volo.Entity.System;
using Ape.Volo.IBusiness.Base;
using Ape.Volo.IBusiness.Dto.System;
using Ape.Volo.IBusiness.QueryModel;

namespace Ape.Volo.IBusiness.Interface.System;

/// <summary>
/// 全局设置接口
/// </summary>
public interface ISettingService : IBaseServices<Setting>
{
    #region 基础接口

    /// <summary>
    /// 创建
    /// </summary>
    /// <param name="createUpdateSettingDto"></param>
    /// <returns></returns>
    Task<bool> CreateAsync(CreateUpdateSettingDto createUpdateSettingDto);

    /// <summary>
    /// 更新
    /// </summary>
    /// <param name="createUpdateSettingDto"></param>
    /// <returns></returns>
    Task<bool> UpdateAsync(CreateUpdateSettingDto createUpdateSettingDto);

    /// <summary>
    /// 删除
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    Task<bool> DeleteAsync(HashSet<long> ids);

    /// <summary>
    /// 查询
    /// </summary>
    /// <param name="settingQueryCriteria"></param>
    /// <param name="pagination"></param>
    /// <returns></returns>
    Task<List<SettingDto>> QueryAsync(SettingQueryCriteria settingQueryCriteria, Pagination pagination);

    /// <summary>
    /// 下载
    /// </summary>
    /// <param name="settingQueryCriteria"></param>
    /// <returns></returns>
    Task<List<ExportBase>> DownloadAsync(SettingQueryCriteria settingQueryCriteria);

    /// <summary>
    /// 根据名称查询
    /// </summary>
    /// <param name="settingName"></param>
    /// <returns></returns>
    Task<T> GetSettingValue<T>(string settingName);

    #endregion
}
