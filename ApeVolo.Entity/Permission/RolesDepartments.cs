using ApeVolo.Common.DI;
using SqlSugar;

namespace ApeVolo.Entity.Permission;

/// <summary>
/// 角色部门关联
/// </summary>
[SugarTable("sys_roles_depts", "角色部门")]
public class RolesDepartments
{
    /// <summary>
    /// 角色ID
    /// </summary>
    [SugarColumn(ColumnDataType = "bigint", IsPrimaryKey = true, ColumnDescription = "角色ID")]
    public long RoleId { get; set; }

    /// <summary>
    /// 部门ID
    /// </summary>
    [SugarColumn(ColumnDataType = "bigint", IsPrimaryKey = true, ColumnDescription = "部门ID")]
    public long DeptId { get; set; }
}
