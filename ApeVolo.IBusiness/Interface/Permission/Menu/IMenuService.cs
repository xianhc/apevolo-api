using System.Collections.Generic;
using System.Threading.Tasks;
using ApeVolo.Common.Helper.Excel;
using ApeVolo.Common.Model;
using ApeVolo.IBusiness.Base;
using ApeVolo.IBusiness.Dto.Permission.Menu;
using ApeVolo.IBusiness.QueryModel;
using ApeVolo.IBusiness.Vo;

namespace ApeVolo.IBusiness.Interface.Permission.Menu;

public interface IMenuService : IBaseServices<Entity.Permission.Menu>
{
    #region 基础接口

    Task<bool> CreateAsync(CreateUpdateMenuDto createUpdateMenuDto);
    Task<bool> UpdateAsync(CreateUpdateMenuDto createUpdateMenuDto);
    Task<bool> DeleteAsync(HashSet<long> ids);
    Task<List<MenuDto>> QueryAsync(MenuQueryCriteria menuQueryCriteria, Pagination pagination);
    Task<List<ExportRowModel>> DownloadAsync(MenuQueryCriteria menuQueryCriteria);

    #endregion

    #region 扩展接口

    /// <summary>
    /// 获取所有菜单
    /// </summary>
    /// <returns></returns>
    Task<List<MenuDto>> QueryAllAsync();

    /// <summary>
    /// 根据父ID获取菜单
    /// </summary>
    /// <param name="pid"></param>
    /// <returns></returns>
    Task<List<MenuDto>> FindByPIdAsync(long pid = 0);

    /// <summary>
    /// 根据角色ID获取菜单
    /// </summary>
    /// <param name="roleId"></param>
    /// <returns></returns>
    Task<List<MenuDto>> FindByRoleIdAsync(long roleId);

    /// <summary>
    /// 构建前端菜单树
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns></returns>
    Task<List<MenuTreeVo>> BuildTreeAsync(long userId);

    /// <summary>
    /// 获取所有同级或父级菜单
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<List<MenuDto>> FindSuperiorAsync(long id);

    /// <summary>
    /// 获取所有子菜单ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<List<long>> FindChildAsync(long id);

    #endregion
}