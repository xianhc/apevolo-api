using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Ape.Volo.Api.Controllers.Base;
using Ape.Volo.Common.AttributeExt;
using Ape.Volo.Common.Extention;
using Ape.Volo.Common.Helper;
using Ape.Volo.Common.Model;
using Ape.Volo.Common.WebApp;
using Ape.Volo.IBusiness.Dto.Permission;
using Ape.Volo.IBusiness.Interface.Permission;
using Ape.Volo.IBusiness.QueryModel;
using Ape.Volo.IBusiness.RequestModel;
using Microsoft.AspNetCore.Mvc;

namespace Ape.Volo.Api.Controllers.Permission;

/// <summary>
/// 菜单管理
/// </summary>
[Area("权限管理")]
[Route("/api/menu")]
public class MenusController : BaseApiController
{
    #region 字段

    private readonly IMenuService _menuService;
    private readonly IHttpUser _httpUser;

    #endregion

    #region 构造函数

    public MenusController(IMenuService menuService, IHttpUser httpUser)
    {
        _menuService = menuService;
        _httpUser = httpUser;
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
    [Description("创建")]
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
    [Description("编辑")]
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
    [Description("删除")]
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
    [Description("构建菜单")]
    [Route("build")]
    //[ApeVoloAuthorize(new[] { "admin", "menu_list", "guest" })]
    [ApeVoloOnline]
    public async Task<ActionResult<object>> Build()
    {
        var menuVos = await _menuService.BuildTreeAsync(_httpUser.Id);
        return menuVos.ToJsonByIgnore();
    }

    /// <summary>
    /// 获取子菜单
    /// </summary>
    /// <param name="pid">父级ID</param>
    /// <returns></returns>
    [HttpGet]
    [Description("子菜单")]
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
    /// <returns></returns>
    [HttpGet]
    [Description("查询")]
    [Route("query")]
    public async Task<ActionResult<object>> Query(MenuQueryCriteria menuQueryCriteria)
    {
        var menuList = await _menuService.QueryAsync(menuQueryCriteria);
        return JsonContent(new ActionResultVm<MenuDto>
        {
            Content = menuList,
            TotalElements = menuList.Count
        });
    }


    /// <summary>
    /// 导出菜单
    /// </summary>
    /// <param name="menuQueryCriteria"></param>
    /// <returns></returns>
    [HttpGet]
    [Description("导出")]
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
    [Description("获取同级、父级菜单")]
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
    [Description("获取所有子级菜单ID")]
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
