using ApeVolo.Common.DI;
using ApeVolo.Entity.Do.Base;
using SqlSugar;

namespace ApeVolo.Entity.Do.Email;

/// <summary>
/// 邮件账户
/// </summary>
[SugarTable("sys_email_account", "邮件账户表")]
public class EmailAccount : EntityRoot<long>, ILocalizedTable
{
    /// <summary>
    ///电子邮件地址
    /// </summary>
    [SugarColumn(ColumnName = "email", ColumnDataType = "varchar", Length = 255, IsNullable = false,
        ColumnDescription = "电子邮件地址")]
    public string Email { get; set; }

    /// <summary>
    /// 电子邮件显示名称
    /// </summary>
    [SugarColumn(ColumnName = "display_name", ColumnDataType = "varchar", Length = 255, IsNullable = false,
        ColumnDescription = "电子邮件显示名称")]
    public string DisplayName { get; set; }

    /// <summary>
    /// 电子邮件主机
    /// </summary>
    [SugarColumn(ColumnName = "host", ColumnDataType = "varchar", Length = 255, IsNullable = false,
        ColumnDescription = "电子邮件主机")]
    public string Host { get; set; }

    /// <summary>
    /// 电子邮件端口
    /// </summary>
    [SugarColumn(ColumnName = "port", ColumnDataType = "int", Length = 11, IsNullable = false,
        ColumnDescription = "电子邮件端口")]
    public int Port { get; set; }

    /// <summary>
    /// 电子邮件用户名
    /// </summary>
    [SugarColumn(ColumnName = "username", ColumnDataType = "varchar", Length = 255, IsNullable = false,
        ColumnDescription = "电子邮件用户名")]
    public string Username { get; set; }

    /// <summary>
    /// 电子邮件密码
    /// </summary>
    [SugarColumn(ColumnName = "password", ColumnDataType = "varchar", Length = 255, IsNullable = true,
        ColumnDescription = "电子邮件密码")]
    public string Password { get; set; }

    /// <summary>
    /// 是否SSL
    /// </summary>
    [SugarColumn(ColumnName = "enable_ssl", ColumnDataType = "bit", Length = 1, IsNullable = false,
        ColumnDescription = "电子邮件用户名")]
    public bool EnableSsl { get; set; }

    /// <summary>
    /// 是否与请求一起发送应用程序的默认系统凭据
    /// </summary>
    [SugarColumn(ColumnName = "use_default_credentials", ColumnDataType = "bit", Length = 1, IsNullable = false,
        ColumnDescription = "是否与请求一起发送应用程序的默认系统凭据")]
    public bool UseDefaultCredentials { get; set; }
}