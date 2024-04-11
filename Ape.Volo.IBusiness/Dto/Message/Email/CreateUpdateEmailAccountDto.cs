using System.ComponentModel.DataAnnotations;
using Ape.Volo.Common.AttributeExt;
using Ape.Volo.Entity.Message.Email;
using Ape.Volo.IBusiness.Base;

namespace Ape.Volo.IBusiness.Dto.Message.Email;

/// <summary>
/// 邮箱账户Dto
/// </summary>
[AutoMapping(typeof(EmailAccount), typeof(CreateUpdateEmailAccountDto))]
public class CreateUpdateEmailAccountDto : BaseEntityDto<long>
{
    /// <summary>
    ///电子邮件地址
    /// </summary>
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    /// <summary>
    /// 电子邮件显示名称
    /// </summary>
    [Required]
    public string DisplayName { get; set; }

    /// <summary>
    /// 电子邮件主机
    /// </summary>
    [Required]
    public string Host { get; set; }

    /// <summary>
    /// 电子邮件端口
    /// </summary>
    [Required]
    public int Port { get; set; }

    /// <summary>
    /// 电子邮件用户名
    /// </summary>
    [Required]
    public string Username { get; set; }

    /// <summary>
    /// 电子邮件密码
    /// </summary>
    [Required]
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
