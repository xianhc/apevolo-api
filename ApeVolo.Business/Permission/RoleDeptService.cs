using System.Collections.Generic;
using System.Threading.Tasks;
using ApeVolo.Business.Base;
using ApeVolo.Entity.Permission;
using ApeVolo.IBusiness.Interface.Permission;
using SqlSugar;

namespace ApeVolo.Business.Permission;

/// <summary>
/// 数据权限
/// </summary>
public class RoleDeptService : BaseServices<RolesDepartments>, IRoleDeptService
{
    #region 基础方法

    public async Task<bool> CreateAsync(List<RolesDepartments> rolesDepartmentses)
    {
        return await SugarRepository.AddReturnBoolAsync(rolesDepartmentses);
    }

    public async Task<bool> DeleteByRoleIdAsync(long roleId)
    {
        return await SugarRepository.DeleteAsync(rd => rd.RoleId == roleId) > 0;
    }

    public async Task<List<RolesDepartments>> QueryByDeptIdsAsync(List<long> deptIds)
    {
        var list =
            await SugarRepository.QueryMuchAsync<RolesDepartments, Role, RolesDepartments>(
                (rd, r) => new object[]
                {
                    JoinType.Left, rd.RoleId == r.Id
                },
                (rd, r) => rd,
                (rd, r) => deptIds.Contains(rd.DeptId)
            );
        return list;
    }

    public async Task<List<RolesDepartments>> QueryByRoleIdAsync(long roleId)
    {
        return await SugarRepository.QueryListAsync(x => x.RoleId == roleId);
    }

    #endregion
}
