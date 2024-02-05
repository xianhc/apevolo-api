using System;
using System.Collections.Generic;

namespace Ape.Volo.IBusiness.QueryModel;

/// <summary>
/// 菜单查询参数
/// </summary>
public class MenuQueryCriteria : DateRange
{
    /// <summary>
    /// 菜单标题
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// 父级ID
    /// </summary>
    public long? ParentId { get; set; }
}
