using ApeVolo.Common.AttributeExt;
using SqlSugar;

namespace ApeVolo.Entity.Do.Core;

[InitTable(typeof(RolesDepartments))]
[SugarTable("sys_roles_depts", "角色部门")]
public class RolesDepartments
{
    [SugarColumn(ColumnName = "role_id", ColumnDataType = "bigint", Length = 19, IsNullable = false,
        IsPrimaryKey = true)]
    public long RoleId { get; set; }

    [SugarColumn(ColumnName = "dept_id", ColumnDataType = "bigint", Length = 19, IsNullable = false,
        IsPrimaryKey = true)]
    public long DeptId { get; set; }
}