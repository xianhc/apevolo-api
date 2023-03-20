using System.Collections.Generic;
using System.Threading.Tasks;
using ApeVolo.Common.Helper.Excel;
using ApeVolo.Common.Model;
using ApeVolo.IBusiness.Base;
using ApeVolo.IBusiness.Dto.Permission.Department;
using ApeVolo.IBusiness.QueryModel;

namespace ApeVolo.IBusiness.Interface.Permission.Department;

/// <summary>
/// 部门接口
/// </summary>
public interface IDepartmentService : IBaseServices<Entity.Permission.Department>
{
    #region 基础接口

    Task<bool> CreateAsync(CreateUpdateDepartmentDto createUpdateDepartmentDto);
    Task<bool> UpdateAsync(CreateUpdateDepartmentDto createUpdateDepartmentDto);
    Task<bool> DeleteAsync(HashSet<long> ids);
    Task<List<DepartmentDto>> QueryAsync(DeptQueryCriteria deptQueryCriteria, Pagination pagination);
    Task<List<ExportRowModel>> DownloadAsync(DeptQueryCriteria deptQueryCriteria);

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
    /// 查找所有子级ID
    /// </summary>
    /// <param name="deptIds"></param>
    /// <param name="departmentDtos"></param>
    /// <returns></returns>
    Task<List<long>> FindChildIds(List<long> deptIds, List<DepartmentDto> departmentDtos);

    #endregion
}