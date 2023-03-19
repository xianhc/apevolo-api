using ApeVolo.Common.DI;
using ApeVolo.Entity.Base;
using SqlSugar;

namespace ApeVolo.Entity.Permission.Role;

/// <summary>
/// 角色部门关联
/// </summary>
[SugarTable("sys_roles_depts", "角色部门")]
public class RolesDepartments : EntityRoot<long>, ILocalizedTable
{
    /// <summary>
    /// 角色ID
    /// </summary>
    [SugarColumn(ColumnName = "role_id", ColumnDataType = "bigint", IsNullable = false,
        ColumnDescription = "角色ID")]
    public long RoleId { get; set; }

    /// <summary>
    /// 部门ID
    /// </summary>
    [SugarColumn(ColumnName = "dept_id", ColumnDataType = "bigint", IsNullable = false,
        ColumnDescription = "部门ID")]
    public long DeptId { get; set; }
}