using ApeVolo.Common.AttributeExt;
using SqlSugar;

namespace ApeVolo.Entity.Do.Core;

[InitTable(typeof(UserRoles))]
[SugarTable("sys_users_roles", "用户角色关联表")]
public class UserRoles
{
    [SugarColumn(ColumnName = "user_id", ColumnDataType = "bigint", Length = 19, IsNullable = false,
        IsPrimaryKey = true)]
    public long UserId { get; set; }

    [SugarColumn(ColumnName = "role_id", ColumnDataType = "bigint", Length = 19, IsNullable = false,
        IsPrimaryKey = true)]
    public long RoleId { get; set; }
}