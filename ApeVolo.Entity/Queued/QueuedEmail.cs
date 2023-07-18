using System;
using ApeVolo.Common.DI;
using ApeVolo.Common.Model;
using ApeVolo.Entity.Base;
using SqlSugar;

namespace ApeVolo.Entity.Queued;

/// <summary>
/// 邮件队列
/// </summary>=
[SugarTable("queued_email", "邮件队列")]
public class QueuedEmail : BaseEntity, ISoftDeletedEntity
{
    /// <summary>
    /// 发件邮箱
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "发件邮箱")]
    public string From { get; set; }

    /// <summary>
    /// 发件人名称
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "发件人名称")]
    public string FromName { get; set; }

    /// <summary>
    /// 收件邮箱
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "收件邮箱")]
    public string To { get; set; }

    /// <summary>
    /// 收件人名称
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "收件人名称")]
    public string ToName { get; set; }

    /// <summary>
    /// 回复邮箱
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "答复邮箱")]
    public string ReplyTo { get; set; }

    /// <summary>
    /// 回复人名称
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "回复人名称")]
    public string ReplyToName { get; set; }

    /// <summary>
    /// 优先级
    /// </summary>
    [SugarColumn(ColumnDataType = "int", IsNullable = false, ColumnDescription = "优先级")]
    public int Priority { get; set; }

    /// <summary>
    /// 抄送
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "抄送")]
    public string Cc { get; set; }

    /// <summary>
    /// 密件抄送
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "密件抄送")]
    public string Bcc { get; set; }

    /// <summary>
    /// 标题
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "标题")]
    public string Subject { get; set; }

    /// <summary>
    /// 内容
    /// </summary>
    [SugarColumn(Length = 5000, IsNullable = false, ColumnDescription = "内容")]
    public string Body { get; set; }

    /// <summary>
    /// 发送上限次数
    /// </summary>
    [SugarColumn(ColumnDataType = "int", IsNullable = false, ColumnDescription = "发送上限次数")]
    public int SentTries { get; set; }

    /// <summary>
    /// 发送时间
    /// </summary>
    [SugarColumn(ColumnDataType = "datetime", IsNullable = true, ColumnDescription = "发送时间")]
    public DateTime? SendTime { get; set; }

    /// <summary>
    /// 发件邮箱ID
    /// </summary>
    [SugarColumn(ColumnDataType = "bigint", IsNullable = false, ColumnDescription = "发件邮箱ID")]
    public long EmailAccountId { get; set; }

    public bool IsDeleted { get; set; }
}
