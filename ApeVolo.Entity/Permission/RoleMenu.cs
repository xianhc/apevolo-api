using ApeVolo.Common.DI;
using SqlSugar;

namespace ApeVolo.Entity.Permission;

/// <summary>
/// 角色菜单关联
/// </summary>
[SugarTable("sys_roles_menus", "角色与菜单关联")]
public class RoleMenu
{
    /// <summary>
    /// 角色ID
    /// </summary>
    [SugarColumn(ColumnDataType = "bigint", IsPrimaryKey = true, ColumnDescription = "角色ID")]
    public long RoleId { get; set; }

    /// <summary>
    /// 菜单ID
    /// </summary>
    [SugarColumn(ColumnDataType = "bigint", IsPrimaryKey = true, ColumnDescription = "菜单ID")]
    public long MenuId { get; set; }
}
