using ApeVolo.Common.AttributeExt;
using SqlSugar;

namespace ApeVolo.Entity.Do.Core;

[InitTable(typeof(Department))]
[SugarTable("sys_department", "部门表")]
public class Department : BaseEntity
{
    [SugarColumn(ColumnName = "name", ColumnDataType = "varchar", Length = 255, IsNullable = false,
        ColumnDescription = "部门名称")]
    public string Name { get; set; }

    [SugarColumn(ColumnName = "parent_id", ColumnDataType = "bigint", Length = 19, IsNullable = true,
        ColumnDescription = "父级部门")]
    public long? PId { get; set; }

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

    //        [SugarColumn(IsIgnore = true)] public bool HasChildren => SubCount > 0;

    //[SugarColumn(IsIgnore = true)] public bool Leaf => SubCount == 0;

    // [SugarColumn(IsIgnore = true)]
    // public List<Department> Children { get; set; }
}