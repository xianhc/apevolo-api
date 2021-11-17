using ApeVolo.Common.Model;
using ApeVolo.IBusiness.Dto.Core;
using ApeVolo.IBusiness.QueryModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApeVolo.IBusiness.Base;
using ApeVolo.IBusiness.EditDto.Core;
using ApeVolo.Common.Helper.Excel;
using ApeVolo.Entity.Do.Core;

namespace ApeVolo.IBusiness.Interface.Core
{
    /// <summary>
    /// 部门接口
    /// </summary>
    public interface IDepartmentService : IBaseServices<Department>
    {
        #region 基础接口
        Task<bool> CreateAsync(CreateUpdateDepartmentDto createUpdateDepartmentDto);
        Task<bool> UpdateAsync(CreateUpdateDepartmentDto createUpdateDepartmentDto);
        Task<bool> DeleteAsync(HashSet<string> ids);
        Task<List<DepartmentDto>> QueryAsync(DeptQueryCriteria deptQueryCriteria, Pagination pagination);
        Task<List<ExportRowModel>> DownloadAsync(DeptQueryCriteria deptQueryCriteria);

        #endregion

        #region 扩展接口

        /// <summary>
        /// 根据父ID获取全部
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<List<DepartmentDto>> QueryByPIdAsync(string id);
        /// <summary>
        /// 根据ID获取一个部门
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<DepartmentSmallDto> QueryByIdAsync(string id);
        
        /// <summary>
        /// 根据角色获取部门
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        Task<List<DepartmentDto>> QueryByRoleIdAsync(string roleId);
        /// <summary>
        /// 获取子级所有部门
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task<List<DepartmentDto>> QuerySuperiorDeptAsync(List<string> ids);
        /// <summary>
        /// 查找所有子级ID
        /// </summary>
        /// <param name="deptIds"></param>
        /// <param name="departmentDtos"></param>
        /// <returns></returns>
        Task<List<string>> FindChildIds(List<string> deptIds, List<DepartmentDto> departmentDtos);

        #endregion
    }
}