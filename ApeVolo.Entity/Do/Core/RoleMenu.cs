using ApeVolo.Common.DI;
using ApeVolo.Entity.Do.Base;
using SqlSugar;

namespace ApeVolo.Entity.Do.Core;

/// <summary>
/// 角色菜单关联
/// </summary>
[SugarTable("sys_roles_menus", "角色与菜单关联")]
public class RoleMenu : EntityRoot<long>, ILocalizedTable
{
    /// <summary>
    /// 角色ID
    /// </summary>
    [SugarColumn(ColumnName = "role_id", ColumnDataType = "bigint", Length = 19, IsNullable = false,
        ColumnDescription = "角色ID")]
    public long RoleId { get; set; }

    /// <summary>
    /// 菜单ID
    /// </summary>
    [SugarColumn(ColumnName = "menu_id", ColumnDataType = "bigint", Length = 19, IsNullable = false,
        ColumnDescription = "菜单ID")]
    public long MenuId { get; set; }
}