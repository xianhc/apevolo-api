using System.Collections.Generic;
using System.Threading.Tasks;
using Ape.Volo.Common.Model;
using Ape.Volo.IBusiness.Base;
using Ape.Volo.IBusiness.Dto.System;
using Ape.Volo.IBusiness.QueryModel;
using ApeVolo.Entity.System;

namespace Ape.Volo.IBusiness.Interface.System;

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
    Task<List<ExportBase>> DownloadAsync(AppsecretQueryCriteria appsecretQueryCriteria);

    #endregion
}
