using System;
using System.Collections.Generic;

namespace Ape.Volo.IBusiness.QueryModel;

/// <summary>
/// 全局设置查询参数
/// </summary>
public class SettingQueryCriteria : DateRange
{
    /// <summary>
    /// 关键字
    /// </summary>
    public string KeyWords { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool? Enabled { get; set; }
}
