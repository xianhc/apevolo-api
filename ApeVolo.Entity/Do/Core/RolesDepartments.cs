using ApeVolo.Common.AttributeExt;
using SqlSugar;

namespace ApeVolo.Entity.Do.Core
{
    [InitTable(typeof(RolesDepartments))]
    [SugarTable("sys_roles_depts", "角色部门")]
    public class RolesDepartments
    {
        [SugarColumn(ColumnName = "role_id", ColumnDataType = "char", Length = 19, IsNullable = false, IsPrimaryKey = true)]
        public string RoleId { get; set; }

        [SugarColumn(ColumnName = "dept_id", ColumnDataType = "char", Length = 19, IsNullable = false, IsPrimaryKey = true)]
        public string DeptId { get; set; }
    }
}
