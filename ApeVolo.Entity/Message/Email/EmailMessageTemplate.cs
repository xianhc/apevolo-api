using ApeVolo.Common.Model;
using ApeVolo.Entity.Base;
using SqlSugar;

namespace ApeVolo.Entity.Message.Email;

/// <summary>
/// 邮件模板
/// </summary>
[SugarTable("email_message_template", "邮件消息模板")]
public class EmailMessageTemplate : BaseEntity, ISoftDeletedEntity
{
    /// <summary>
    /// 模板名称
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "模板名称")]
    public string Name { get; set; }

    /// <summary>
    /// 抄送邮箱地址
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "抄送邮箱地址")]
    public string BccEmailAddresses { get; set; }

    /// <summary>
    /// 主题
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "主题")]
    public string Subject { get; set; }

    /// <summary>
    /// 内容
    /// </summary>
    [SugarColumn(Length = 4000, IsNullable = false, ColumnDescription = "内容")]
    public string Body { get; set; }

    /// <summary>
    /// 是否激活
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "是否激活")]
    public bool IsActive { get; set; }

    /// <summary>
    /// 邮箱账户标识符
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "邮箱账户标识符")]
    public long EmailAccountId { get; set; }

    public bool IsDeleted { get; set; }
}
