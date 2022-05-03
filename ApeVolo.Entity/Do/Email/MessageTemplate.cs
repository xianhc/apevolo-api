using ApeVolo.Common.DI;
using ApeVolo.Entity.Do.Base;
using SqlSugar;

namespace ApeVolo.Entity.Do.Email;

/// <summary>
/// 邮件模板
/// </summary>
[SugarTable("sys_email_message_template", "邮件消息模板")]
public class MessageTemplate : EntityRoot<long>, ILocalizedTable
{
    /// <summary>
    /// 模板名称
    /// </summary>
    [SugarColumn(ColumnName = "name", ColumnDataType = "varchar", Length = 255, IsNullable = false,
        ColumnDescription = "模板名称")]
    public string Name { get; set; }

    /// <summary>
    /// 抄送邮箱地址
    /// </summary>
    [SugarColumn(ColumnName = "bcc_email_addresses", ColumnDataType = "varchar", Length = 255, IsNullable = true,
        ColumnDescription = "抄送邮箱地址")]
    public string BccEmailAddresses { get; set; }

    /// <summary>
    /// 主题
    /// </summary>
    [SugarColumn(ColumnName = "subject", ColumnDataType = "varchar", Length = 1000, IsNullable = true,
        ColumnDescription = "主题")]
    public string Subject { get; set; }

    /// <summary>
    /// 内容
    /// </summary>
    [SugarColumn(ColumnName = "body", ColumnDataType = "varchar", Length = 5000, IsNullable = false,
        ColumnDescription = "内容")]
    public string Body { get; set; }

    /// <summary>
    /// 是否激活
    /// </summary>
    [SugarColumn(ColumnName = "is_active", ColumnDataType = "bit", Length = 1, IsNullable = false,
        ColumnDescription = "是否激活")]
    public bool IsActive { get; set; }

    /// <summary>
    /// 邮箱账户标识符
    /// </summary>
    [SugarColumn(ColumnName = "email_account_id", ColumnDataType = "varchar", Length = 255, IsNullable = false,
        ColumnDescription = "邮箱账户标识符")]
    public string EmailAccountId { get; set; }
}