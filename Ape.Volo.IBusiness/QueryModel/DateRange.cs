using System;
using System.Collections.Generic;

namespace Ape.Volo.IBusiness.QueryModel;

/// <summary>
/// 日期范围
/// </summary>
public class DateRange
{
    /// <summary>
    /// 开始[0]--结束[1]
    /// </summary>
    public List<DateTime> CreateTime { get; set; }
}
