using System;
using System.Collections.Generic;

namespace Ape.Volo.IBusiness.QueryModel;

/// <summary>
/// 任务调度查询参数
/// </summary>
public class QuartzNetQueryCriteria : DateRange
{
    /// <summary>
    /// 任务名称
    /// </summary>
    public string TaskName { get; set; }
}
