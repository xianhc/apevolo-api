using System.Collections.Generic;
using System.Threading.Tasks;
using ApeVolo.Business.Base;
using ApeVolo.Entity.Do.Core;
using ApeVolo.IBusiness.Interface.Core;
using ApeVolo.IRepository.Core;
using SqlSugar;

namespace ApeVolo.Business.Impl.Core;

/// <summary>
/// 数据权限
/// </summary>
public class RoleDeptService : BaseServices<RolesDepartments>, IRoleDeptService
{
    #region 字段

    #endregion

    #region 构造函数

    public RoleDeptService(IRoleDeptRepository repository)
    {
        BaseDal = repository;
    }

    #endregion

    #region 基础方法

    public async Task<bool> CreateAsync(List<RolesDepartments> rolesDepartmentses)
    {
        return await BaseDal.AddReturnBoolAsync(rolesDepartmentses);
    }

    public async Task<bool> DeleteByRoleIdAsync(long roleId)
    {
        return await BaseDal.DeleteAsync(rd => rd.RoleId == roleId) > 0;
    }

    public async Task<List<RolesDepartments>> QueryByDeptIdsAsync(List<long> deptIds)
    {
        var list = await BaseDal.QueryMuchAsync<RolesDepartments, Role, RolesDepartments>(
            (rd, r) => new object[]
            {
                JoinType.Left, rd.RoleId == r.Id
            },
            (rd, r) => rd,
            (rd, r) => r.IsDeleted == false && deptIds.Contains(rd.DeptId)
        );
        return list;
    }

    public async Task<List<RolesDepartments>> QueryByRoleIdAsync(long roleId)
    {
        return await BaseDal.QueryListAsync(x => x.RoleId == roleId);
    }

    #endregion
}