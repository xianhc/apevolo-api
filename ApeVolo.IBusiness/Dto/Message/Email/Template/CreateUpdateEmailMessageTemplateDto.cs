using System.ComponentModel.DataAnnotations;
using ApeVolo.Common.AttributeExt;
using ApeVolo.Entity.Message.Email;
using ApeVolo.IBusiness.Base;

namespace ApeVolo.IBusiness.Dto.Message.Email.Template;

[AutoMapping(typeof(EmailMessageTemplate), typeof(CreateUpdateEmailMessageTemplateDto))]
public class CreateUpdateEmailMessageTemplateDto : EntityDtoRoot<long>
{
    /// <summary>
    /// 模板名称
    /// </summary>
    [Display(Name = "EmailMessageTemplate.Name")]
    [Required(ErrorMessage = "{0}required")]
    public string Name { get; set; }

    /// <summary>
    /// 抄送邮箱地址
    /// </summary>
    public string BccEmailAddresses { get; set; }

    /// <summary>
    /// 主题
    /// </summary>
    [Display(Name = "EmailMessageTemplate.Subject")]
    [Required(ErrorMessage = "{0}required")]
    public string Subject { get; set; }

    /// <summary>
    /// 内容
    /// </summary>
    [Display(Name = "EmailMessageTemplate.Body")]
    [Required(ErrorMessage = "{0}required")]
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