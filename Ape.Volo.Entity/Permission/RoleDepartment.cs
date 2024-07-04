using SqlSugar;

namespace Ape.Volo.Entity.Permission;

/// <summary>
/// 角色部门关联
/// </summary>
[SugarTable("sys_role_dept")]
public class RoleDepartment
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
