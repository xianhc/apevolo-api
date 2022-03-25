using System.Collections.Generic;
using ApeVolo.Common.AttributeExt;
using SqlSugar;

namespace ApeVolo.Entity.Do.Core;

[InitTable(typeof(Role))]
[SugarTable("sys_role", "角色表")]
public class Role : BaseEntity
{
    [SugarColumn(ColumnName = "name", ColumnDataType = "varchar", Length = 255, IsNullable = false,
        ColumnDescription = "角色名称")]
    public string Name { get; set; }

    [SugarColumn(ColumnName = "level", IsNullable = false, ColumnDescription = "角色等级")]
    public int Level { get; set; }

    [SugarColumn(ColumnName = "description", ColumnDataType = "varchar", Length = 255, IsNullable = true,
        ColumnDescription = "描述")]
    public string Description { get; set; }

    [SugarColumn(ColumnName = "data_scope", ColumnDataType = "varchar", Length = 50, IsNullable = false,
        ColumnDescription = "数据权限")]
    public string DataScope { get; set; }

    [SugarColumn(ColumnName = "permission", ColumnDataType = "varchar", Length = 20, IsNullable = false,
        ColumnDescription = "角色代码")]
    public string Permission { get; set; }

    [SugarColumn(IsIgnore = true)] public List<Menu> MenuList { get; set; }

    [SugarColumn(IsIgnore = true)] public List<Department> DepartmentList { get; set; }
}