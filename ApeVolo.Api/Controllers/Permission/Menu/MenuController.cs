using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using ApeVolo.Api.ActionExtension.Json;
using ApeVolo.Api.Controllers.Base;
using ApeVolo.Common.AttributeExt;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Helper.Excel;
using ApeVolo.Common.Model;
using ApeVolo.Common.Resources;
using ApeVolo.Common.WebApp;
using ApeVolo.IBusiness.Dto.Permission.Menu;
using ApeVolo.IBusiness.Interface.Permission.Menu;
using ApeVolo.IBusiness.QueryModel;
using ApeVolo.IBusiness.RequestModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace ApeVolo.Api.Controllers.Permission.Menu;

/// <summary>
/// 菜单管理
/// </summary>
[Area("Menu")]
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
    [Description("{0}Add")]
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
    [Description("{0}Edit")]
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
    /// <param name="collection"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route("delete")]
    [Description("{0}Delete")]
    [NoJsonParamter]
    public async Task<ActionResult<object>> Delete([FromBody] IdCollection collection)
    {
        if (!ModelState.IsValid)
        {
            var actionError = ModelState.GetErrors();
            return Error(actionError);
        }

        await _menuService.DeleteAsync(collection.IdArray);
        return Success();
    }

    /// <summary>
    /// 构建树形菜单
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Description("BuildMenu")]
    [Route("build")]
    [ApeVoloAuthorize(new[] { "admin", "menu_list", "guest" })]
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
    [Description("{0}List")]
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
    [Description("{0}Export")]
    [Route("download")]
    public async Task<ActionResult<object>> Download(MenuQueryCriteria menuQueryCriteria)
    {
        var exportRowModels = await _menuService.DownloadAsync(menuQueryCriteria);

        var filepath = ExcelHelper.ExportData(exportRowModels, Localized.Get("Menu"));

        FileInfo fileInfo = new FileInfo(filepath);
        var ext = fileInfo.Extension;
        new FileExtensionContentTypeProvider().Mappings.TryGetValue(ext, out var contently);
        return File(await global::System.IO.File.ReadAllBytesAsync(filepath), contently ?? "application/octet-stream",
            fileInfo.Name);
    }

    /// <summary>
    /// 获取同级与上级菜单
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    [HttpPost]
    [Description("SiblingParentMenu")]
    [Route("superior")]
    [NoJsonParamter]
    [ApeVoloAuthorize(new[] { "admin", "menu_list" })]
    public async Task<ActionResult<object>> GetSuperior([FromBody] List<long> ids)
    {
        if (!ModelState.IsValid)
        {
            var actionError = ModelState.GetErrors();
            return Error(actionError);
        }

        var menuVos = await _menuService.FindSuperiorAsync(ids[0]);
        return menuVos.ToJsonByIgnore();
    }

    [HttpGet]
    [Description("AllChildID")]
    [Route("child")]
    [ApeVoloAuthorize(new[] { "admin", "menu_list" })]
    [NoJsonParamter]
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