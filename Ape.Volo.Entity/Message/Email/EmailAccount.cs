using Ape.Volo.Common.Model;
using Ape.Volo.Entity.Base;
using SqlSugar;

namespace Ape.Volo.Entity.Message.Email;

/// <summary>
/// 邮件账户
/// </summary>
[SugarTable("email_account")]
public class EmailAccount : BaseEntity
{
    /// <summary>
    ///电子邮件地址
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public string Email { get; set; }

    /// <summary>
    /// 电子邮件显示名称
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public string DisplayName { get; set; }

    /// <summary>
    /// 电子邮件主机
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public string Host { get; set; }

    /// <summary>
    /// 电子邮件端口
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public int Port { get; set; }

    /// <summary>
    /// 电子邮件用户名
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public string Username { get; set; }

    /// <summary>
    /// 电子邮件密码
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public string Password { get; set; }

    /// <summary>
    /// 是否SSL
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public bool EnableSsl { get; set; }

    /// <summary>
    /// 是否与请求一起发送应用程序的默认系统凭据
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public bool UseDefaultCredentials { get; set; }
}
