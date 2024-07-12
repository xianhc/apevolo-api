using Ape.Volo.Entity.Base;
using SqlSugar;

namespace Ape.Volo.Entity.Message.Email;

/// <summary>
/// 邮件模板
/// </summary>
[SugarTable("email_message_template")]
public class EmailMessageTemplate : BaseEntity
{
    /// <summary>
    /// 模板名称
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public string Name { get; set; }

    /// <summary>
    /// 抄送邮箱地址
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public string BccEmailAddresses { get; set; }

    /// <summary>
    /// 主题
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public string Subject { get; set; }

    /// <summary>
    /// 内容
    /// </summary>
    [SugarColumn(Length = 4000, IsNullable = false)]
    public string Body { get; set; }

    /// <summary>
    /// 是否激活
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public bool IsActive { get; set; }

    /// <summary>
    /// 邮箱账户标识符
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public long EmailAccountId { get; set; }
}
