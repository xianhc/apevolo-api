using SqlSugar;

namespace Ape.Volo.Entity.Permission
{
    /// <summary>
    /// 角色部门关联
    /// </summary>
    [SugarTable("sys_roles_depts")]
    public class RolesDepartments
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public long RoleId { get; set; }

        /// <summary>
        /// 部门ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public long DeptId { get; set; }
    }
}
