using System.Collections.Generic;
using ApeVolo.Common.DI;
using ApeVolo.Common.Model;
using ApeVolo.Entity.Base;
using SqlSugar;

namespace ApeVolo.Entity.Permission;

/// <summary>
/// 角色
/// </summary>
[SugarTable("sys_role", "角色")]
public class Role : BaseEntity, ISoftDeletedEntity
{
    /// <summary>
    /// 角色名称
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "角色名称")]
    public string Name { get; set; }

    /// <summary>
    /// 角色等级
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "角色等级")]
    public int Level { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "描述")]
    public string Description { get; set; }

    /// <summary>
    /// 数据权限
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = false, ColumnDescription = "数据权限")]
    public string DataScope { get; set; }

    /// <summary>
    /// 角色代码
    /// </summary>
    [SugarColumn(Length = 20, IsNullable = false, ColumnDescription = "角色代码")]
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

    public bool IsDeleted { get; set; }
}
