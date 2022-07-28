using ApeVolo.Common.AttributeExt;
using ApeVolo.Entity.Do.Email;
using ApeVolo.IBusiness.Dto;

namespace ApeVolo.IBusiness.EditDto.Email;

[AutoMapping(typeof(EmailMessageTemplate), typeof(CreateUpdateEmailMessageTemplateDto))]
public class CreateUpdateEmailMessageTemplateDto : EntityDtoRoot<long>
{
    /// <summary>
    /// 模板名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 抄送邮箱地址
    /// </summary>
    public string BccEmailAddresses { get; set; }

    /// <summary>
    /// 主题
    /// </summary>
    public string Subject { get; set; }

    /// <summary>
    /// 内容
    /// </summary>
    public string Body { get; set; }

    /// <summary>
    /// 是否激活
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// 邮箱账户标识符
    /// </summary>
    public string EmailAccountId { get; set; }
}