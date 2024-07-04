using SqlSugar;

namespace Ape.Volo.Entity.Permission;

/// <summary>
/// 角色菜单关联
/// </summary>
[SugarTable("sys_role_menu")]
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
