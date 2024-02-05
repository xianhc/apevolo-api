using System.Collections.Generic;
using System.Threading.Tasks;
using Ape.Volo.Common.Model;
using Ape.Volo.Entity.Message.Email;
using Ape.Volo.IBusiness.Base;
using Ape.Volo.IBusiness.Dto.Message.Email;
using Ape.Volo.IBusiness.QueryModel;

namespace Ape.Volo.IBusiness.Interface.Message.Email;

/// <summary>
/// 邮箱账户接口
/// </summary>
public interface IEmailAccountService : IBaseServices<EmailAccount>
{
    #region 基础接口

    /// <summary>
    /// 创建
    /// </summary>
    /// <param name="createUpdateEmailAccountDto"></param>
    /// <returns></returns>
    Task<bool> CreateAsync(CreateUpdateEmailAccountDto createUpdateEmailAccountDto);

    /// <summary>
    /// 更新
    /// </summary>
    /// <param name="createUpdateEmailAccountDto"></param>
    /// <returns></returns>
    Task<bool> UpdateAsync(CreateUpdateEmailAccountDto createUpdateEmailAccountDto);

    /// <summary>
    /// 删除
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    Task<bool> DeleteAsync(HashSet<long> ids);

    /// <summary>
    /// 查询
    /// </summary>
    /// <param name="emailAccountQueryCriteria"></param>
    /// <param name="pagination"></param>
    /// <returns></returns>
    Task<List<EmailAccountDto>> QueryAsync(EmailAccountQueryCriteria emailAccountQueryCriteria,
        Pagination pagination);

    /// <summary>
    /// 下载
    /// </summary>
    /// <param name="emailAccountQueryCriteria"></param>
    /// <returns></returns>
    Task<List<ExportBase>> DownloadAsync(EmailAccountQueryCriteria emailAccountQueryCriteria);

    #endregion
}
