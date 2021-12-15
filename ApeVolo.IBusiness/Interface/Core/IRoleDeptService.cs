using System.Collections.Generic;
using System.Threading.Tasks;
using ApeVolo.Entity.Do.Core;
using ApeVolo.IBusiness.Base;

namespace ApeVolo.IBusiness.Interface.Core
{
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
}