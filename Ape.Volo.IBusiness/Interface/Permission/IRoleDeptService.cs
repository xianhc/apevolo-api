using System.Collections.Generic;
using System.Threading.Tasks;
using Ape.Volo.Entity.Permission;
using Ape.Volo.IBusiness.Base;

namespace Ape.Volo.IBusiness.Interface.Permission;

/// <summary>
/// 角色部门
/// </summary>
public interface IRoleDeptService : IBaseServices<RolesDepartments>
{
    #region 基础接口

    Task<bool> CreateAsync(List<RolesDepartments> rolesDepartmentses);
    Task<bool> DeleteByRoleIdAsync(long roleId);
    Task<List<RolesDepartments>> QueryByDeptIdsAsync(List<long> deptIds);
    Task<List<RolesDepartments>> QueryByRoleIdAsync(long roleId);

    #endregion
}
