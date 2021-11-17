using ApeVolo.Common.Model;
using ApeVolo.IBusiness.Base;
using ApeVolo.IBusiness.Dto.Email;
using ApeVolo.IBusiness.EditDto.Email;
using ApeVolo.IBusiness.QueryModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApeVolo.Entity.Do.Email;

namespace ApeVolo.IBusiness.Interface.Email
{
    /// <summary>
    /// 邮件消息模板接口
    /// </summary>
    public interface IEmailMessageTemplateService : IBaseServices<MessageTemplate>
    {
        #region 基础接口
        Task<bool> CreateAsync(CreateUpdateMessageTemplateDto createUpdateMessageTemplateDto);

        Task<bool> UpdateAsync(CreateUpdateMessageTemplateDto createUpdateMessageTemplateDto);

        Task<bool> DeleteAsync(HashSet<string> ids);

        Task<List<MessageTemplateDto>> QueryAsync(MessageTemplateQueryCriteria messageTemplateQueryCriteria, Pagination pagination);
        #endregion
    }
}
