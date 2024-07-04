using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Ape.Volo.Api.Controllers.Base;
using Ape.Volo.Common.Extensions;
using Ape.Volo.Common.Helper;
using Ape.Volo.Common.Model;
using Ape.Volo.IBusiness.Dto.Permission;
using Ape.Volo.IBusiness.Interface.Permission;
using Microsoft.AspNetCore.Mvc;

namespace Ape.Volo.Api.Controllers.Permission;

/// <summary>
/// 角色授权管理
/// </summary>
[Area("角色授权管理")]
[Route("/api/permissions", Order = 3)]
public class PermissionsController : BaseApiController
{
    #region 字段

    private readonly IRoleService _roleService;
    private readonly IMenuService _menuService;
    private readonly IApisService _apisService;

    #endregion

    #region 构造函数

    public PermissionsController(IRoleService roleService, IMenuService menuService, IApisService apisService)
    {
        _roleService = roleService;
        _menuService = menuService;
        _apisService = apisService;
    }

    #endregion


    #region 对内接口

    /// <summary>
    /// 查询Apis
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("menus/query")]
    [Description("查询菜单")]
    public async Task<ActionResult<object>> QueryAllMenus()
    {
        var menus = await _menuService.QueryAllAsync();

        var menuTree = TreeHelper<MenuDto>.ListToTrees(menus, "Id", "ParentId", 0);
        return menuTree.ToJson();
    }


    /// <summary>
    /// 查询Apis
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("apis/query")]
    [Description("查询Apis")]
    public async Task<ActionResult<object>> QueryAllApis()
    {
        List<ApisTree> apisTree = new List<ApisTree>();
        var apis = await _apisService.QueryAllAsync();
        var apisGroup = apis.GroupBy(x => x.Group).ToList();

        var index = 0;
        foreach (var g in apisGroup)
        {
            var apisTreesTmp = new List<ApisTree>();
            foreach (var api in g.ToList())
            {
                apisTreesTmp.Add(new ApisTree()
                {
                    Id = api.Id,
                    Label = api.Description,
                    Leaf = true,
                    HasChildren = false,
                    Children = null
                });
            }

            index++;
            apisTree.Add(new ApisTree()
            {
                Id = index,
                Label = g.Key,
                Leaf = false,
                HasChildren = true,
                Children = apisTreesTmp
            });
        }

        return apisTree.ToJson();
    }


    /// <summary>
    /// 更新角色菜单关联
    /// </summary>
    /// <param name="createUpdateRoleDto"></param>
    /// <returns></returns>
    [HttpPut]
    [Route("menus/edit")]
    [Description("编辑角色菜单")]
    public async Task<ActionResult<object>> UpdateRolesMenus(CreateUpdateRoleDto createUpdateRoleDto)
    {
        await _roleService.UpdateRolesMenusAsync(createUpdateRoleDto);
        return NoContent();
    }

    /// <summary>
    /// 更新角色Api关联
    /// </summary>
    /// <param name="createUpdateRoleDto"></param>
    /// <returns></returns>
    [HttpPut]
    [Route("apis/edit")]
    [Description("编辑角色Apis")]
    public async Task<ActionResult<object>> UpdateRolesApis(CreateUpdateRoleDto createUpdateRoleDto)
    {
        await _roleService.UpdateRolesApisAsync(createUpdateRoleDto);
        return NoContent();
    }

    #endregion
}
