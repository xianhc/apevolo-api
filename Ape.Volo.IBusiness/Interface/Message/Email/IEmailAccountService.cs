using System.Collections.Generic;
using System.Threading.Tasks;
using Ape.Volo.Common.Model;
using Ape.Volo.IBusiness.Base;
using Ape.Volo.IBusiness.Dto.Message.Email;
using Ape.Volo.IBusiness.QueryModel;
using ApeVolo.Entity.Message.Email;

namespace Ape.Volo.IBusiness.Interface.Message.Email;

/// <summary>
/// 邮箱账户接口
/// </summary>
public interface IEmailAccountService : IBaseServices<EmailAccount>
{
    #region 基础接口

    Task<bool> CreateAsync(CreateUpdateEmailAccountDto createUpdateEmailAccountDto);

    Task<bool> UpdateAsync(CreateUpdateEmailAccountDto createUpdateEmailAccountDto);

    Task<bool> DeleteAsync(HashSet<long> ids);

    Task<List<EmailAccountDto>> QueryAsync(EmailAccountQueryCriteria emailAccountQueryCriteria,
        Pagination pagination);

    Task<List<ExportBase>> DownloadAsync(EmailAccountQueryCriteria emailAccountQueryCriteria);

    #endregion
}
