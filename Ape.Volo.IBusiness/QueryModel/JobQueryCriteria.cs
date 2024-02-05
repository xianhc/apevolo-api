using System;
using System.Collections.Generic;

namespace Ape.Volo.IBusiness.QueryModel;

/// <summary>
/// 岗位查询参数
/// </summary>
public class JobQueryCriteria : DateRange
{
    /// <summary>
    /// 岗位名称
    /// </summary>
    public string JobName { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool? Enabled { get; set; }
}
