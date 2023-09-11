using ApeVolo.Entity.Base;
using SqlSugar;

namespace ApeVolo.Entity.Permission;

/// <summary>
/// 用户角色关联
/// </summary>
[SugarTable("sys_users_roles")]
public class UserRoles
{
    /// <summary>
    /// 用户ID
    /// </summary>
    [SugarColumn(IsPrimaryKey = true)]
    public long UserId { get; set; }

    /// <summary>
    /// 角色ID
    /// </summary>
    [SugarColumn(IsPrimaryKey = true)]
    public long RoleId { get; set; }
}
