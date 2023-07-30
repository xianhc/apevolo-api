using SqlSugar;

namespace ApeVolo.Entity.Permission;

/// <summary>
/// 用户角色关联
/// </summary>
[SugarTable("sys_users_roles", "用户角色关联")]
public class UserRoles
{
    /// <summary>
    /// 用户ID
    /// </summary>
    [SugarColumn(IsPrimaryKey = true, ColumnDescription = "用户ID")]
    public long UserId { get; set; }

    /// <summary>
    /// 角色ID
    /// </summary>
    [SugarColumn(IsPrimaryKey = true, ColumnDescription = "角色ID")]
    public long RoleId { get; set; }
}
