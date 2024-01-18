using Ape.Volo.Common.AttributeExt;
using Ape.Volo.IBusiness.Base;
using ApeVolo.Entity.Message.Email;

namespace Ape.Volo.IBusiness.Dto.Message.Email;

[AutoMapping(typeof(EmailMessageTemplate), typeof(EmailMessageTemplateDto))]
public class EmailMessageTemplateDto : BaseEntityDto<long>
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
    /// 发送邮箱账户
    /// </summary>
    public long EmailAccountId { get; set; }
}
