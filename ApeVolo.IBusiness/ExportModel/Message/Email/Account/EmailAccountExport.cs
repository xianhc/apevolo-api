using System.ComponentModel.DataAnnotations;
using ApeVolo.Common.Global;
using ApeVolo.Common.Model;

namespace ApeVolo.IBusiness.ExportModel.Message.Email.Account;

public class EmailAccountExport : ExportBase
{
    /// <summary>
    ///电子邮件地址
    /// </summary>
    [Display(Name = "EmailAccount.Email")]
    public string Email { get; set; }

    /// <summary>
    /// 电子邮件显示名称
    /// </summary>
    [Display(Name = "电子邮件")]
    public string DisplayName { get; set; }

    /// <summary>
    /// 电子邮件主机
    /// </summary>
    [Display(Name = "主机")]
    public string Host { get; set; }

    /// <summary>
    /// 电子邮件端口
    /// </summary>
    [Display(Name = "端口")]
    public int Port { get; set; }

    /// <summary>
    /// 电子邮件用户名
    /// </summary>
    [Display(Name = "账户名称")]
    public string Username { get; set; }

    /// <summary>
    /// 电子邮件密码
    /// </summary>
    [Display(Name = "密码")]
    public string Password { get; set; }

    /// <summary>
    /// 是否SSL
    /// </summary>
    [Display(Name = "是否SSL")]
    public BoolState EnableSsl { get; set; }

    /// <summary>
    /// 是否与请求一起发送应用程序的默认系统凭据
    /// </summary>
    [Display(Name = "发送默认系统凭据")]
    public BoolState UseDefaultCredentials { get; set; }
}
