using System;
using SqlSugar;

namespace ApeVolo.Entity.Base;

/// <summary>
/// 实体基类
/// </summary>
public class BaseEntity : RootKey<long>
{
    /// <summary>
    /// 创建者名称
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "创建者账号")]
    public string CreateBy { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "创建时间")]
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// 更新者名称
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "更新者账户")]
    public string UpdateBy { get; set; }

    /// <summary>
    /// 最后更新时间
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "更新时间")]
    public DateTime? UpdateTime { get; set; }
}
