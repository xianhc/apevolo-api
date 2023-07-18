using System;
using ApeVolo.Common.AttributeExt;
using ApeVolo.Entity.Queued;
using ApeVolo.IBusiness.Base;

namespace ApeVolo.IBusiness.Dto.Queued;

[AutoMapping(typeof(QueuedEmail), typeof(CreateUpdateQueuedEmailDto))]
public class CreateUpdateQueuedEmailDto : BaseEntityDto<long>
{
    /// <summary>
    /// 发件邮箱
    /// </summary>
    public string From { get; set; }

    /// <summary>
    /// 发件人名称
    /// </summary>
    public string FromName { get; set; }

    /// <summary>
    /// 收件邮箱
    /// </summary>
    public string To { get; set; }

    /// <summary>
    /// 收件人名称
    /// </summary>
    public string ToName { get; set; }

    /// <summary>
    /// 回复邮箱
    /// </summary>
    public string ReplyTo { get; set; }

    /// <summary>
    /// 回复人名称
    /// </summary>
    public string ReplyToName { get; set; }

    /// <summary>
    /// 优先级
    /// </summary>
    public int Priority { get; set; }

    /// <summary>
    /// 抄送
    /// </summary>
    public string CC { get; set; }

    /// <summary>
    /// 密件抄送
    /// </summary>
    public string Bcc { get; set; }

    /// <summary>
    /// 标题
    /// </summary>
    public string Subject { get; set; }

    /// <summary>
    /// 内容
    /// </summary>
    public string Body { get; set; }

    /// <summary>
    /// 发送上限次数
    /// </summary>
    public int SentTries { get; set; }

    /// <summary>
    /// 发送时间
    /// </summary>
    public DateTime? SendTime { get; set; }

    /// <summary>
    /// 发件邮箱ID
    /// </summary>
    public string EmailAccountId { get; set; }
}
