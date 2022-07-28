using ApeVolo.Common.DI;
using ApeVolo.Entity.Do.Base;
using SqlSugar;

namespace ApeVolo.Entity.Do.Core;

/// <summary>
/// 部门
/// </summary>
[SugarTable("sys_department", "部门")]
public class Department : EntityRoot<long>, ILocalizedTable
{
    /// <summary>
    /// 部门名称
    /// </summary>
    [SugarColumn(ColumnName = "name", IsNullable = false, ColumnDescription = "部门名称")]
    public string Name { get; set; }

    /// <summary>
    /// 父级部门ID
    /// </summary>
    [SugarColumn(ColumnName = "parent_id", ColumnDataType = "bigint", IsNullable = true, ColumnDescription = "父级部门")]
    public long? PId { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    [SugarColumn(ColumnName = "sort", IsNullable = true, ColumnDescription = "排序")]
    public int Sort { get; set; }

    /// <summary>
    /// 是否激活
    /// </summary>
    [SugarColumn(ColumnName = "enabled", IsNullable = true, ColumnDescription = "是否启用")]
    public bool Enabled { get; set; }

    /// <summary>
    /// 子节点个数
    /// </summary>
    [SugarColumn(ColumnName = "sub_count", IsNullable = true, ColumnDescription = "子节点个数")]
    public int SubCount { get; set; }
}