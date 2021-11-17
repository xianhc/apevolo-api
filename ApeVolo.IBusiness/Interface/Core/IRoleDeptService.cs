using ApeVolo.IBusiness.Base;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApeVolo.Entity.Do.Core;

namespace ApeVolo.IBusiness.Interface.Core
{
    /// <summary>
    /// 角色部门
    /// </summary>
    public interface IRoleDeptService : IBaseServices<RolesDepartments>
    {
        #region 基础接口
        Task<bool> CreateAsync(List<RolesDepartments> rolesDepartmentses);
        Task<bool> DeleteByRoleIdAsync(string roleId);
        Task<List<RolesDepartments>> QueryByDeptIdsAsync(List<string> deptIds);
        Task<List<RolesDepartments>> QueryByRoleIdAsync(string roleId);
        #endregion
    }
}