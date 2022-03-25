using System.Collections.Generic;
using System.Threading.Tasks;
using ApeVolo.Common.Helper.Excel;
using ApeVolo.Common.Model;
using ApeVolo.Entity.Do.Email;
using ApeVolo.IBusiness.Base;
using ApeVolo.IBusiness.Dto.Email;
using ApeVolo.IBusiness.EditDto.Email;
using ApeVolo.IBusiness.QueryModel;

namespace ApeVolo.IBusiness.Interface.Email;

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

    Task<List<ExportRowModel>> DownloadAsync(EmailAccountQueryCriteria emailAccountQueryCriteria);

    #endregion
}