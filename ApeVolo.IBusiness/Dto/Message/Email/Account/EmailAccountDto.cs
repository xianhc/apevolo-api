using System.ComponentModel.DataAnnotations;
using ApeVolo.Common.AttributeExt;
using ApeVolo.Entity.Do.Email;
using ApeVolo.IBusiness.Base;

namespace ApeVolo.IBusiness.Dto.Message.Email.Account;

[AutoMapping(typeof(EmailAccount), typeof(EmailAccountDto))]
public class EmailAccountDto : EntityDtoRoot<long>
{
    /// <summary>
    ///电子邮件地址
    /// </summary>
    [Display(Name = "EmailAccount.Email")]
    public string Email { get; set; }

    /// <summary>
    /// 电子邮件显示名称
    /// </summary>
    [Display(Name = "EmailAccount.DisplayName")]
    public string DisplayName { get; set; }

    /// <summary>
    /// 电子邮件主机
    /// </summary>
    [Display(Name = "EmailAccount.Host")]
    public string Host { get; set; }

    /// <summary>
    /// 电子邮件端口
    /// </summary>
    [Display(Name = "EmailAccount.Port")]
    public int Port { get; set; }

    /// <summary>
    /// 电子邮件用户名
    /// </summary>
    [Display(Name = "EmailAccount.Username")]
    public string Username { get; set; }

    /// <summary>
    /// 电子邮件密码
    /// </summary>
    [Display(Name = "EmailAccount.Password")]
    public string Password { get; set; }

    /// <summary>
    /// 是否SSL
    /// </summary>
    [Display(Name = "EmailAccount.EnableSsl")]
    public bool EnableSsl { get; set; }

    /// <summary>
    /// 是否与请求一起发送应用程序的默认系统凭据
    /// </summary>
    [Display(Name = "EmailAccount.UseDefaultCredentials")]
    public bool UseDefaultCredentials { get; set; }
}