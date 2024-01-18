using System.Collections.Generic;
using System.Threading.Tasks;
using Ape.Volo.Common.Model;
using Ape.Volo.IBusiness.Base;
using Ape.Volo.IBusiness.Dto.Permission;
using Ape.Volo.IBusiness.QueryModel;
using ApeVolo.Entity.Permission;

namespace Ape.Volo.IBusiness.Interface.Permission;

/// <summary>
/// 部门接口
/// </summary>
public interface IDepartmentService : IBaseServices<Department>
{
    #region 基础接口

    Task<bool> CreateAsync(CreateUpdateDepartmentDto createUpdateDepartmentDto);
    Task<bool> UpdateAsync(CreateUpdateDepartmentDto createUpdateDepartmentDto);
    Task<bool> DeleteAsync(List<long> ids);
    Task<List<DepartmentDto>> QueryAsync(DeptQueryCriteria deptQueryCriteria, Pagination pagination);
    Task<List<ExportBase>> DownloadAsync(DeptQueryCriteria deptQueryCriteria);

    #endregion

    #region 扩展接口

    /// <summary>
    /// 根据父ID获取全部
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<List<DepartmentDto>> QueryByPIdAsync(long id);

    /// <summary>
    /// 根据ID获取一个部门
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<DepartmentSmallDto> QueryByIdAsync(long id);

    /// <summary>
    /// 根据角色获取部门
    /// </summary>
    /// <param name="roleId"></param>
    /// <returns></returns>
    Task<List<DepartmentDto>> QueryByRoleIdAsync(long roleId);

    /// <summary>
    /// 获取子级所有部门
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    Task<List<DepartmentDto>> QuerySuperiorDeptAsync(HashSet<long> ids);

    /// <summary>
    /// 获取所选部门及全部下级部门ID
    /// </summary>
    /// <param name="ids"></param>
    /// <param name="allIds"></param>
    /// <returns></returns>
    Task<List<long>> GetChildIds(List<long> ids, List<long> allIds);

    #endregion
}
