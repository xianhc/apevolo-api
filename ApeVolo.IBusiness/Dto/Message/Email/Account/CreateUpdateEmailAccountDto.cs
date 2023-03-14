using System.ComponentModel.DataAnnotations;
using ApeVolo.Common.AttributeExt;
using ApeVolo.Entity.Do.Email;
using ApeVolo.IBusiness.Base;

namespace ApeVolo.IBusiness.Dto.Message.Email.Account;

[AutoMapping(typeof(EmailAccount), typeof(CreateUpdateEmailAccountDto))]
public class CreateUpdateEmailAccountDto : EntityDtoRoot<long>
{
    /// <summary>
    ///电子邮件地址
    /// </summary>
    [Display(Name = "Email")]
    [Required(ErrorMessage = "{0}required")]
    [EmailAddress(ErrorMessage = "{0}ValueIsInvalidAccessor")]
    public string Email { get; set; }

    /// <summary>
    /// 电子邮件显示名称
    /// </summary>
    [Display(Name = "Email.DisplayName")]
    [Required(ErrorMessage = "{0}required")]
    public string DisplayName { get; set; }

    /// <summary>
    /// 电子邮件主机
    /// </summary>
    [Display(Name = "Email.Host")]
    [Required(ErrorMessage = "{0}required")]
    public string Host { get; set; }

    /// <summary>
    /// 电子邮件端口
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    /// 电子邮件用户名
    /// </summary>
    [Display(Name = "Email.Username")]
    [Required(ErrorMessage = "{0}required")]
    public string Username { get; set; }

    /// <summary>
    /// 电子邮件密码
    /// </summary>
    [Display(Name = "Email.Password")]
    [Required(ErrorMessage = "{0}required")]
    public string Password { get; set; }

    /// <summary>
    /// 是否SSL
    /// </summary>
    public bool EnableSsl { get; set; } = false;

    /// <summary>
    /// 是否与请求一起发送应用程序的默认系统凭据
    /// </summary>
    public bool UseDefaultCredentials { get; set; } = false;
}