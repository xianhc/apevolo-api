using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using ApeVolo.Api.Controllers.Base;
using ApeVolo.Common.AttributeExt;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Helper;
using ApeVolo.Common.Model;
using ApeVolo.Common.WebApp;
using ApeVolo.IBusiness.Dto.Permission.Menu;
using ApeVolo.IBusiness.Interface.Permission.Menu;
using ApeVolo.IBusiness.QueryModel;
using ApeVolo.IBusiness.RequestModel;
using Microsoft.AspNetCore.Mvc;

namespace ApeVolo.Api.Controllers.Permission.Menu;

/// <summary>
/// 菜单管理
/// </summary>
[Area("Permission")]
[Route("/api/menu")]
public class MenusController : BaseApiController
{
    #region 字段

    private readonly IMenuService _menuService;
    private readonly ICurrentUser _currentUser;

    #endregion

    #region 构造函数

    public MenusController(IMenuService menuService, ICurrentUser currentUser)
    {
        _menuService = menuService;
        _currentUser = currentUser;
    }

    #endregion

    #region 内部接口

    /// <summary>
    /// 新增菜单
    /// </summary>
    /// <param name="createUpdateMenuDto"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("create")]
    [Description("Add")]
    public async Task<ActionResult<object>> CreateMenu(
        [FromBody] CreateUpdateMenuDto createUpdateMenuDto)
    {
        if (!ModelState.IsValid)
        {
            var actionError = ModelState.GetErrors();
            return Error(actionError);
        }

        await _menuService.CreateAsync(createUpdateMenuDto);
        return Create();
    }

    /// <summary>
    /// 更新菜单
    /// </summary>
    /// <param name="createUpdateMenuDto"></param>
    /// <returns></returns>
    [HttpPut]
    [Route("edit")]
    [Description("Edit")]
    public async Task<ActionResult<object>> UpdateDept(
        [FromBody] CreateUpdateMenuDto createUpdateMenuDto)
    {
        if (!ModelState.IsValid)
        {
            var actionError = ModelState.GetErrors();
            return Error(actionError);
        }

        await _menuService.UpdateAsync(createUpdateMenuDto);
        return NoContent();
    }

    /// <summary>
    /// 删除菜单
    /// </summary>
    /// <param name="idCollection"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route("delete")]
    [Description("Delete")]
    public async Task<ActionResult<object>> Delete([FromBody] IdCollection idCollection)
    {
        if (!ModelState.IsValid)
        {
            var actionError = ModelState.GetErrors();
            return Error(actionError);
        }

        await _menuService.DeleteAsync(idCollection.IdArray);
        return Success();
    }

    /// <summary>
    /// 构建树形菜单
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Description("BuildMenu")]
    [Route("build")]
    //[ApeVoloAuthorize(new[] { "admin", "menu_list", "guest" })]
    [ApeVoloOnline]
    public async Task<ActionResult<object>> Build()
    {
        var menuVos = await _menuService.BuildTreeAsync(_currentUser.Id);
        return menuVos.ToJsonByIgnore();
    }

    /// <summary>
    /// 获取子菜单
    /// </summary>
    /// <param name="pid">父级ID</param>
    /// <returns></returns>
    [HttpGet]
    [Description("Submenu")]
    [Route("lazy")]
    [ApeVoloAuthorize(new[] { "admin", "menu_list", "guest" })]
    public async Task<ActionResult<object>> GetMenuLazy(long pid)
    {
        if (pid.IsNullOrEmpty())
        {
            return Error("pid is null");
        }

        var menulist = await _menuService.FindByPIdAsync(pid);
        return menulist.ToJsonByIgnore();
    }

    /// <summary>
    /// 查看菜单列表
    /// </summary>
    /// <param name="menuQueryCriteria"></param>
    /// <param name="pagination"></param>
    /// <returns></returns>
    [HttpGet]
    [Description("List")]
    [Route("query")]
    public async Task<ActionResult<object>> Query(MenuQueryCriteria menuQueryCriteria,
        Pagination pagination)
    {
        var menuList = await _menuService.QueryAsync(menuQueryCriteria, pagination);
        return new ActionResultVm<MenuDto>
        {
            Content = menuList,
            TotalElements = menuList.Count
        }.ToJson();
    }


    /// <summary>
    /// 导出菜单
    /// </summary>
    /// <param name="menuQueryCriteria"></param>
    /// <returns></returns>
    [HttpGet]
    [Description("Export")]
    [Route("download")]
    public async Task<ActionResult<object>> Download(MenuQueryCriteria menuQueryCriteria)
    {
        var menuExports = await _menuService.DownloadAsync(menuQueryCriteria);
        var data = new ExcelHelper().GenerateExcel(menuExports, out var mimeType);
        return File(data, mimeType);
    }

    /// <summary>
    /// 获取同级与上级菜单
    /// </summary>
    /// <param name="idCollection"></param>
    /// <returns></returns>
    [HttpPost]
    [Description("SiblingParentMenu")]
    [Route("superior")]
    [ApeVoloAuthorize(new[] { "admin", "menu_list" })]
    public async Task<ActionResult<object>> GetSuperior([FromBody] IdCollection idCollection)
    {
        if (!ModelState.IsValid)
        {
            var actionError = ModelState.GetErrors();
            return Error(actionError);
        }

        var menuVos = await _menuService.FindSuperiorAsync(idCollection.IdArray.FirstOrDefault());
        return menuVos.ToJsonByIgnore();
    }

    [HttpGet]
    [Description("AllChildID")]
    [Route("child")]
    [ApeVoloAuthorize(new[] { "admin", "menu_list" })]
    public async Task<ActionResult<object>> GetChild(long id)
    {
        if (id.IsNullOrEmpty())
        {
            return Error("id is null");
        }

        var menuIds = await _menuService.FindChildAsync(id);
        return menuIds.ToJson();
    }

    #endregion
}