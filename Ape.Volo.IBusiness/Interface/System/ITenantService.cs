using System.Collections.Generic;
using System.Threading.Tasks;
using Ape.Volo.Common.Model;
using Ape.Volo.Entity.System;
using Ape.Volo.IBusiness.Base;
using Ape.Volo.IBusiness.Dto.System;
using Ape.Volo.IBusiness.QueryModel;

namespace Ape.Volo.IBusiness.Interface.System;

/// <summary>
/// 租户接口
/// </summary>
public interface ITenantService : IBaseServices<Tenant>
{
    #region 基础接口

    /// <summary>
    /// 创建
    /// </summary>
    /// <param name="createUpdateTenantDtoDto"></param>
    /// <returns></returns>
    Task<bool> CreateAsync(CreateUpdateTenantDto createUpdateTenantDtoDto);

    /// <summary>
    /// 更新
    /// </summary>
    /// <param name="createUpdateTenantDtoDto"></param>
    /// <returns></returns>
    Task<bool> UpdateAsync(CreateUpdateTenantDto createUpdateTenantDtoDto);

    /// <summary>
    /// 删除
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    Task<bool> DeleteAsync(HashSet<long> ids);

    /// <summary>
    /// 查询
    /// </summary>
    /// <param name="tenantQueryCriteria"></param>
    /// <param name="pagination"></param>
    /// <returns></returns>
    Task<List<TenantDto>> QueryAsync(TenantQueryCriteria tenantQueryCriteria, Pagination pagination);


    /// <summary>
    /// 查询全部
    /// </summary>
    /// <returns></returns>
    Task<List<TenantDto>> QueryAllAsync();

    /// <summary>
    /// 下载
    /// </summary>
    /// <param name="tenantQueryCriteria"></param>
    /// <returns></returns>
    Task<List<ExportBase>> DownloadAsync(TenantQueryCriteria tenantQueryCriteria);

    #endregion
}
