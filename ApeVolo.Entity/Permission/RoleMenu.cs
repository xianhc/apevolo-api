using SqlSugar;

namespace ApeVolo.Entity.Permission;

/// <summary>
/// 角色菜单关联
/// </summary>
[SugarTable("sys_roles_menus")]
public class RoleMenu
{
    /// <summary>
    /// 角色ID
    /// </summary>
    [SugarColumn(IsPrimaryKey = true)]
    public long RoleId { get; set; }

    /// <summary>
    /// 菜单ID
    /// </summary>
    [SugarColumn(IsPrimaryKey = true)]
    public long MenuId { get; set; }
}
