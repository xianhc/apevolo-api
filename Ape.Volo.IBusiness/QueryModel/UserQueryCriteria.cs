using System;
using System.Collections.Generic;

namespace Ape.Volo.IBusiness.QueryModel;

/// <summary>
/// 用户查询参数
/// </summary>
public class UserQueryCriteria : DateRange
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 部门ID集合
    /// </summary>
    public List<long> DeptIds { get; set; }

    /// <summary>
    /// 关键字
    /// </summary>
    public string KeyWords { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool? Enabled { get; set; }

    /// <summary>
    /// 部门ID
    /// </summary>
    public long DeptId { get; set; }
}
