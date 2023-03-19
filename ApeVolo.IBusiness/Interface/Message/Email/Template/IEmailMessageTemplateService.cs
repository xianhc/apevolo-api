using System.Collections.Generic;
using System.Threading.Tasks;
using ApeVolo.Common.Model;
using ApeVolo.Entity.Message.Email;
using ApeVolo.IBusiness.Base;
using ApeVolo.IBusiness.Dto.Message.Email.Template;
using ApeVolo.IBusiness.QueryModel;

namespace ApeVolo.IBusiness.Interface.Message.Email.Template;

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