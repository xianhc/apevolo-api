using ApeVolo.Common.Helper.Excel;
using ApeVolo.Common.Model;
using ApeVolo.IBusiness.Base;
using ApeVolo.IBusiness.Dto.Core;
using ApeVolo.IBusiness.EditDto.Core;
using ApeVolo.IBusiness.QueryModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApeVolo.Entity.Do.Core;

namespace ApeVolo.IBusiness.Interface.Core
{
    /// <summary>
    /// 角色接口
    /// </summary>
    public interface IRoleService : IBaseServices<Role>
    {
        #region 基础接口
        Task<bool> CreateAsync(CreateUpdateRoleDto createUpdateRoleDto);
        Task<bool> UpdateAsync(CreateUpdateRoleDto createUpdateRoleDto);
        Task<bool> DeleteAsync(HashSet<string> ids);
        Task<List<RoleDto>> QueryAsync(RoleQueryCriteria roleQueryCriteria, Pagination pagination);
        Task<List<ExportRowModel>> DownloadAsync(RoleQueryCriteria userQueryCriteria);
        #endregion

        #region 扩展接口
        /// <summary>
        /// 获取单个角色
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        Task<List<RoleDto>> QuerySingleAsync(string roleId);
        /// <summary>
        /// 根据用户ID获取角色
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<List<RoleSmallDto>> QueryByUserIdAsync(string id);

        /// <summary>
        /// 获取全部角色
        /// </summary>
        /// <returns></returns>
        Task<List<RoleDto>> QueryAllAsync();

        /// <summary>
        /// 获取用户角色等级
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task<int> QueryUserRoleLevelAsync(HashSet<string> ids);

        /// <summary>
        /// 验证角色等级
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        Task<int> VerificationUserRoleLevelAsync(int? level);
        /// <summary>
        /// 更新角色菜单
        /// </summary>
        /// <param name="createUpdateRoleDto"></param>
        /// <returns></returns>
        Task<bool> UpdateRolesMenusAsync(CreateUpdateRoleDto createUpdateRoleDto);
        #endregion
    }
}