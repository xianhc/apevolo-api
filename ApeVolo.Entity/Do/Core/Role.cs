using System.Collections.Generic;
using ApeVolo.Common.DI;
using ApeVolo.Entity.Do.Base;
using SqlSugar;

namespace ApeVolo.Entity.Do.Core;

/// <summary>
/// 角色
/// </summary>
[SugarTable("sys_role", "角色")]
public class Role : EntityRoot<long>, ILocalizedTable
{
    /// <summary>
    /// 角色名称
    /// </summary>
    [SugarColumn(ColumnName = "name", IsNullable = false, ColumnDescription = "角色名称")]
    public string Name { get; set; }

    /// <summary>
    /// 角色等级
    /// </summary>
    [SugarColumn(ColumnName = "level", IsNullable = false, ColumnDescription = "角色等级")]
    public int Level { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    [SugarColumn(ColumnName = "description", IsNullable = true, ColumnDescription = "描述")]
    public string Description { get; set; }

    /// <summary>
    /// 数据权限
    /// </summary>
    [SugarColumn(ColumnName = "data_scope", ColumnDataType = "nvarchar", Length = 50, IsNullable = false,
        ColumnDescription = "数据权限")]
    public string DataScope { get; set; }

    /// <summary>
    /// 角色代码
    /// </summary>
    [SugarColumn(ColumnName = "permission", ColumnDataType = "nvarchar", Length = 20, IsNullable = false,
        ColumnDescription = "角色代码")]
    public string Permission { get; set; }

    /// <summary>
    /// 菜单集合
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public List<Menu> MenuList { get; set; }

    /// <summary>
    /// 部门集合
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public List<Department> DepartmentList { get; set; }
}