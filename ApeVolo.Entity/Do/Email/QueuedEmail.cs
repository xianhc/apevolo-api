using System;
using ApeVolo.Common.AttributeExt;
using SqlSugar;

namespace ApeVolo.Entity.Do.Email;

/// <summary>
/// 邮件队列
/// </summary>
[InitTable(typeof(QueuedEmail))]
[SugarTable("sys_queued_email", "邮件队列")]
public class QueuedEmail : BaseEntity
{
    /// <summary>
    /// 发件邮箱
    /// </summary>
    [SugarColumn(ColumnName = "from", ColumnDataType = "varchar", Length = 255, IsNullable = false,
        ColumnDescription = "发件邮箱")]
    public string From { get; set; }

    /// <summary>
    /// 发件人名称
    /// </summary>
    [SugarColumn(ColumnName = "from_name", ColumnDataType = "varchar", Length = 255, IsNullable = true,
        ColumnDescription = "发件人名称")]
    public string FromName { get; set; }

    /// <summary>
    /// 收件邮箱
    /// </summary>
    [SugarColumn(ColumnName = "to", ColumnDataType = "varchar", Length = 255, IsNullable = false,
        ColumnDescription = "收件邮箱")]
    public string To { get; set; }

    /// <summary>
    /// 收件人名称
    /// </summary>
    [SugarColumn(ColumnName = "to_name", ColumnDataType = "varchar", Length = 255, IsNullable = true,
        ColumnDescription = "收件人名称")]
    public string ToName { get; set; }

    /// <summary>
    /// 回复邮箱
    /// </summary>
    [SugarColumn(ColumnName = "reply_to", ColumnDataType = "varchar", Length = 255, IsNullable = true,
        ColumnDescription = "答复邮箱")]
    public string ReplyTo { get; set; }

    /// <summary>
    /// 回复人名称
    /// </summary>
    [SugarColumn(ColumnName = "reply_to_name", ColumnDataType = "int", Length = 11, IsNullable = true,
        ColumnDescription = "回复人名称")]
    public string ReplyToName { get; set; }

    /// <summary>
    /// 优先级
    /// </summary>
    [SugarColumn(ColumnName = "priority", ColumnDataType = "int", Length = 11, IsNullable = false,
        ColumnDescription = "优先级")]
    public int Priority { get; set; }

    /// <summary>
    /// 抄送
    /// </summary>
    [SugarColumn(ColumnName = "cc", ColumnDataType = "varchar", Length = 255, IsNullable = true,
        ColumnDescription = "抄送")]
    public string CC { get; set; }

    /// <summary>
    /// 密件抄送
    /// </summary>
    [SugarColumn(ColumnName = "bcc", ColumnDataType = "varchar", Length = 255, IsNullable = true,
        ColumnDescription = "密件抄送")]
    public string Bcc { get; set; }

    /// <summary>
    /// 标题
    /// </summary>
    [SugarColumn(ColumnName = "subject", ColumnDataType = "varchar", Length = 255, IsNullable = true,
        ColumnDescription = "标题")]
    public string Subject { get; set; }

    /// <summary>
    /// 内容
    /// </summary>
    [SugarColumn(ColumnName = "body", ColumnDataType = "varchar", Length = 5000, IsNullable = false,
        ColumnDescription = "内容")]
    public string Body { get; set; }

    /// <summary>
    /// 发送上限次数
    /// </summary>
    [SugarColumn(ColumnName = "send_tries", ColumnDataType = "int", Length = 11, IsNullable = false,
        ColumnDescription = "发送上限次数")]
    public int SentTries { get; set; }

    /// <summary>
    /// 发送时间
    /// </summary>
    [SugarColumn(ColumnName = "send_time", ColumnDataType = "datetime", IsNullable = true, ColumnDescription = "发送时间")]
    public DateTime? SendTime { get; set; }

    /// <summary>
    /// 发件邮箱ID
    /// </summary>
    [SugarColumn(ColumnName = "email_account_id", ColumnDataType = "bigint", Length = 19, IsNullable = false,
        ColumnDescription = "发件邮箱ID")]
    public long EmailAccountId { get; set; }
}