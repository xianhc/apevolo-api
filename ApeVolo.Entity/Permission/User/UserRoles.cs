using ApeVolo.Common.DI;
using ApeVolo.Entity.Base;
using SqlSugar;

namespace ApeVolo.Entity.Permission.User;

/// <summary>
/// 用户角色关联
/// </summary>
[SugarTable("sys_users_roles", "用户角色关联")]
public class UserRoles : EntityRoot<long>, ILocalizedTable
{
    /// <summary>
    /// 用户ID
    /// </summary>
    [SugarColumn(ColumnName = "user_id", ColumnDataType = "bigint", IsNullable = false,
        ColumnDescription = "用户ID")]
    public long UserId { get; set; }

    /// <summary>
    /// 角色ID
    /// </summary>
    [SugarColumn(ColumnName = "role_id", ColumnDataType = "bigint", IsNullable = false,
        ColumnDescription = "角色ID")]
    public long RoleId { get; set; }
}