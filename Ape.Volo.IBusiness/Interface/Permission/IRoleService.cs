using System.Collections.Generic;
using System.Threading.Tasks;
using Ape.Volo.Common.Model;
using Ape.Volo.Entity.Permission;
using Ape.Volo.IBusiness.Base;
using Ape.Volo.IBusiness.Dto.Permission;
using Ape.Volo.IBusiness.QueryModel;

namespace Ape.Volo.IBusiness.Interface.Permission;

/// <summary>
/// 角色接口
/// </summary>
public interface IRoleService : IBaseServices<Role>
{
    #region 基础接口

    Task<bool> CreateAsync(CreateUpdateRoleDto createUpdateRoleDto);
    Task<bool> UpdateAsync(CreateUpdateRoleDto createUpdateRoleDto);
    Task<bool> DeleteAsync(HashSet<long> ids);
    Task<List<RoleDto>> QueryAsync(RoleQueryCriteria roleQueryCriteria, Pagination pagination);
    Task<List<ExportBase>> DownloadAsync(RoleQueryCriteria roleQueryCriteria);

    #endregion

    #region 扩展接口

    /// <summary>
    /// 根据用户ID获取角色
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<List<RoleSmallDto>> QueryByUserIdAsync(long id);

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
    Task<int> QueryUserRoleLevelAsync(HashSet<long> ids);

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
