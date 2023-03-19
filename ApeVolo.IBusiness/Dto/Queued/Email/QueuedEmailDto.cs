using System;
using System.ComponentModel.DataAnnotations;
using ApeVolo.Common.AttributeExt;
using ApeVolo.Entity.Queued;
using ApeVolo.IBusiness.Base;

namespace ApeVolo.IBusiness.Dto.Queued.Email;

[AutoMapping(typeof(QueuedEmail), typeof(QueuedEmailDto))]
public class QueuedEmailDto : EntityDtoRoot<long>
{
    /// <summary>
    /// 发件邮箱
    /// </summary>
    [Display(Name = "QueuedEmail.From")]
    public string From { get; set; }

    /// <summary>
    /// 发件人名称
    /// </summary>
    [Display(Name = "QueuedEmail.FromName")]
    public string FromName { get; set; }

    /// <summary>
    /// 收件邮箱
    /// </summary>
    [Display(Name = "QueuedEmail.To")]
    public string To { get; set; }

    /// <summary>
    /// 收件人名称
    /// </summary>
    [Display(Name = "QueuedEmail.ToName")]
    public string ToName { get; set; }

    /// <summary>
    /// 回复邮箱
    /// </summary>
    [Display(Name = "QueuedEmail.ReplyTo")]
    public string ReplyTo { get; set; }

    /// <summary>
    /// 回复人名称
    /// </summary>
    [Display(Name = "QueuedEmail.ReplyToName")]
    public string ReplyToName { get; set; }

    /// <summary>
    /// 优先级
    /// </summary>
    [Display(Name = "QueuedEmail.Priority")]
    public int Priority { get; set; }

    /// <summary>
    /// 抄送
    /// </summary>
    [Display(Name = "QueuedEmail.CC")]
    public string CC { get; set; }

    /// <summary>
    /// 密件抄送
    /// </summary>
    [Display(Name = "QueuedEmail.Bcc")]
    public string Bcc { get; set; }

    /// <summary>
    /// 标题
    /// </summary>
    [Display(Name = "QueuedEmail.Subject")]
    public string Subject { get; set; }

    /// <summary>
    /// 内容
    /// </summary>
    [Display(Name = "QueuedEmail.Body")]
    public string Body { get; set; }

    /// <summary>
    /// 发送上限次数
    /// </summary>
    [Display(Name = "QueuedEmail.SentTries")]
    public int SentTries { get; set; }

    /// <summary>
    /// 发送时间
    /// </summary>
    [Display(Name = "QueuedEmail.SendTime")]
    public DateTime? SendTime { get; set; }

    /// <summary>
    /// 发件邮箱ID
    /// </summary>
    [Display(Name = "QueuedEmail.EmailAccountId")]
    public long EmailAccountId { get; set; }
}