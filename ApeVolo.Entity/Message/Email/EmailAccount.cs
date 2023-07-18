using ApeVolo.Common.DI;
using ApeVolo.Common.Model;
using ApeVolo.Entity.Base;
using SqlSugar;

namespace ApeVolo.Entity.Message.Email;

/// <summary>
/// 邮件账户
/// </summary>
[SugarTable("email_account", "邮件账户表")]
public class EmailAccount : BaseEntity, ISoftDeletedEntity
{
    /// <summary>
    ///电子邮件地址
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "电子邮件地址")]
    public string Email { get; set; }

    /// <summary>
    /// 电子邮件显示名称
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "电子邮件显示名称")]
    public string DisplayName { get; set; }

    /// <summary>
    /// 电子邮件主机
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "电子邮件主机")]
    public string Host { get; set; }

    /// <summary>
    /// 电子邮件端口
    /// </summary>
    [SugarColumn(ColumnDataType = "int", IsNullable = false, ColumnDescription = "电子邮件端口")]
    public int Port { get; set; }

    /// <summary>
    /// 电子邮件用户名
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "电子邮件用户名")]
    public string Username { get; set; }

    /// <summary>
    /// 电子邮件密码
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "电子邮件密码")]
    public string Password { get; set; }

    /// <summary>
    /// 是否SSL
    /// </summary>
    [SugarColumn(ColumnDataType = "bit", IsNullable = false, ColumnDescription = "电子邮件用户名")]
    public bool EnableSsl { get; set; }

    /// <summary>
    /// 是否与请求一起发送应用程序的默认系统凭据
    /// </summary>
    [SugarColumn(ColumnDataType = "bit", IsNullable = false, ColumnDescription = "是否与请求一起发送应用程序的默认系统凭据")]
    public bool UseDefaultCredentials { get; set; }

    public bool IsDeleted { get; set; }
}
