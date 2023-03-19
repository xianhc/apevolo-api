using System.ComponentModel.DataAnnotations;
using ApeVolo.Common.AttributeExt;
using ApeVolo.Entity.Message.Email;
using ApeVolo.IBusiness.Base;

namespace ApeVolo.IBusiness.Dto.Message.Email.Template;

[AutoMapping(typeof(EmailMessageTemplate), typeof(EmailMessageTemplateDto))]
public class EmailMessageTemplateDto : EntityDtoRoot<long>
{
    /// <summary>
    /// 模板名称
    /// </summary>
    [Display(Name = "EmailTemplate.Name")]
    public string Name { get; set; }

    /// <summary>
    /// 抄送邮箱地址
    /// </summary>
    [Display(Name = "EmailTemplate.BccEmailAddresses")]
    public string BccEmailAddresses { get; set; }

    /// <summary>
    /// 主题
    /// </summary>
    [Display(Name = "EmailTemplate.Subject")]
    public string Subject { get; set; }

    /// <summary>
    /// 内容
    /// </summary>
    [Display(Name = "EmailTemplate.Body")]
    public string Body { get; set; }

    /// <summary>
    /// 是否激活
    /// </summary>
    [Display(Name = "EmailTemplate.IsActive")]
    public bool IsActive { get; set; }

    /// <summary>
    /// 发送邮箱账户
    /// </summary>
    [Display(Name = "EmailTemplate.EmailAccountId")]
    public string EmailAccountId { get; set; }
}