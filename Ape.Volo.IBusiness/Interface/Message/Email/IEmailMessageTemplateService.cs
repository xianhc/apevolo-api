using System.Collections.Generic;
using System.Threading.Tasks;
using Ape.Volo.Common.Model;
using Ape.Volo.Entity.Message.Email;
using Ape.Volo.IBusiness.Base;
using Ape.Volo.IBusiness.Dto.Message.Email;
using Ape.Volo.IBusiness.QueryModel;

namespace Ape.Volo.IBusiness.Interface.Message.Email;

/// <summary>
/// 邮件消息模板接口
/// </summary>
public interface IEmailMessageTemplateService : IBaseServices<EmailMessageTemplate>
{
    #region 基础接口

    Task<bool> CreateAsync(CreateUpdateEmailMessageTemplateDto createUpdateEmailMessageTemplateDto);

    Task<bool> UpdateAsync(CreateUpdateEmailMessageTemplateDto createUpdateEmailMessageTemplateDto);

    Task<bool> DeleteAsync(HashSet<long> ids);

    Task<List<EmailMessageTemplateDto>> QueryAsync(EmailMessageTemplateQueryCriteria messageTemplateQueryCriteria,
        Pagination pagination);

    #endregion
}
