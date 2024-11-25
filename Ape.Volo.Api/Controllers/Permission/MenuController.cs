using System.ComponentModel;
using System.Threading.Tasks;
using Ape.Volo.Api.Controllers.Base;
using Ape.Volo.Common;
using Ape.Volo.Common.Extensions;
using Ape.Volo.Common.Helper;
using Ape.Volo.Common.Model;
using Ape.Volo.IBusiness.Dto.Permission;
using Ape.Volo.IBusiness.Interface.Permission;
using Ape.Volo.IBusiness.QueryModel;
using Ape.Volo.IBusiness.RequestModel;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Ape.Volo.Api.Controllers.Permission;

/// <summary>
/// 菜单管理
/// </summary>
[Area("菜单管理")]
[Route("/api/menu", Order = 4)]
public class MenusController : BaseApiController
{
    #region 字段

    private readonly IMenuService _menuService;

    #endregion

    #region 构造函数

    public MenusController(IMenuService menuService)
    {
        _menuService = menuService;
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
    public async Task<ActionResult> Create(
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
    public async Task<ActionResult> Update(
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
    public async Task<ActionResult> Delete([FromBody] IdCollection idCollection)
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
    public async Task<ActionResult<object>> Build()
    {
        var menuVos = await _menuService.BuildTreeAsync(App.HttpUser.Id);
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
    public async Task<ActionResult<object>> GetMenuLazy(long pid)
    {
        if (pid.IsNullOrEmpty())
        {
            return Error("pid cannot be empty");
        }

        var menuList = await _menuService.FindByPIdAsync(pid);
        return menuList.ToJsonByIgnore();
    }

    /// <summary>
    /// 查看菜单列表
    /// </summary>
    /// <param name="menuQueryCriteria"></param>
    /// <returns></returns>
    [HttpGet]
    [Description("查询")]
    [Route("query")]
    public async Task<ActionResult> Query(MenuQueryCriteria menuQueryCriteria)
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
    public async Task<ActionResult> Download(MenuQueryCriteria menuQueryCriteria)
    {
        var menuExports = await _menuService.DownloadAsync(menuQueryCriteria);
        var data = new ExcelHelper().GenerateExcel(menuExports, out var mimeType, out var fileName);
        return new FileContentResult(data, mimeType)
        {
            FileDownloadName = fileName
        };
    }

    /// <summary>
    /// 获取同级与上级菜单
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet]
    [Description("获取同级、父级菜单")]
    [Route("superior")]
    public async Task<ActionResult<object>> GetSuperior(long id)
    {
        if (id.IsNullOrEmpty())
        {
            return Error("id cannot be empty");
        }

        var menuVos = await _menuService.FindSuperiorAsync(id);
        return menuVos.ToJsonByIgnore();
    }

    [HttpGet]
    [Description("获取所有子级菜单ID")]
    [Route("child")]
    public async Task<ActionResult> GetChild(long id)
    {
        if (id.IsNullOrEmpty())
        {
            return Error("id is null");
        }

        var menuIds = await _menuService.FindChildAsync(id);
        return Ok(menuIds);
    }

    #endregion
}
