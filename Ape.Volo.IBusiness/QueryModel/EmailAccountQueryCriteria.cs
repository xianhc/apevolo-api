using System;
using System.Collections.Generic;

namespace Ape.Volo.IBusiness.QueryModel;

/// <summary>
/// 邮箱账户查询参数
/// </summary>
public class EmailAccountQueryCriteria : DateRange
{
    /// <summary>
    /// 邮箱
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// 现实名称
    /// </summary>
    public string DisplayName { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    public string Username { get; set; }
}
