using System;
using ApeVolo.Common.DI;
using ApeVolo.Common.Model;
using ApeVolo.Entity.Base;
using SqlSugar;

namespace ApeVolo.Entity.Permission;

/// <summary>
/// 部门
/// </summary>
[SugarTable("sys_department", "部门")]
public class Department : BaseEntity, ISoftDeletedEntity
{
    /// <summary>
    /// 部门名称
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "部门名称")]
    public string Name { get; set; }

    /// <summary>
    /// 父级部门ID
    /// </summary>
    [SugarColumn(ColumnDataType = "bigint", IsNullable = true, ColumnDescription = "父级部门")]
    public long? ParentId { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "排序")]
    public int Sort { get; set; }

    /// <summary>
    /// 是否激活
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "是否启用")]
    public bool Enabled { get; set; }

    /// <summary>
    /// 子节点个数
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "子节点个数")]
    public int SubCount { get; set; }

    public bool IsDeleted { get; set; }
}
