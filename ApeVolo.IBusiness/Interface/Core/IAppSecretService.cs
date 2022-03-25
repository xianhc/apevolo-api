using System.Collections.Generic;
using System.Threading.Tasks;
using ApeVolo.Common.Helper.Excel;
using ApeVolo.Common.Model;
using ApeVolo.Entity.Do.Core;
using ApeVolo.IBusiness.Base;
using ApeVolo.IBusiness.Dto.Core;
using ApeVolo.IBusiness.EditDto.Core;
using ApeVolo.IBusiness.QueryModel;

namespace ApeVolo.IBusiness.Interface.Core;

/// <summary>
/// 应用秘钥
/// </summary>
public interface IAppSecretService : IBaseServices<AppSecret>
{
    #region 基础接口

    Task<bool> CreateAsync(CreateUpdateAppSecretDto createUpdateAppSecretDto);
    Task<bool> UpdateAsync(CreateUpdateAppSecretDto createUpdateAppSecretDto);
    Task<bool> DeleteAsync(HashSet<long> ids);
    Task<List<AppSecretDto>> QueryAsync(AppsecretQueryCriteria appsecretQueryCriteria, Pagination pagination);
    Task<List<ExportRowModel>> DownloadAsync(AppsecretQueryCriteria appsecretQueryCriteria);

    #endregion
}