using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using ApeVolo.Api.ActionExtension.Json;
using ApeVolo.Api.Controllers.Base;
using ApeVolo.Common.AttributeExt;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Helper;
using ApeVolo.Common.Helper.Excel;
using ApeVolo.Common.Model;
using ApeVolo.Common.WebApp;
using ApeVolo.IBusiness.Dto.Core;
using ApeVolo.IBusiness.EditDto.Core;
using ApeVolo.IBusiness.Interface.Core;
using ApeVolo.IBusiness.QueryModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace ApeVolo.Api.Controllers
{
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
        [Description("新增菜单")]
        public async Task<ActionResult<object>> CreateMenu(
            [FromBody] CreateUpdateMenuDto createUpdateMenuDto)
        {
            RequiredHelper.IsValid(createUpdateMenuDto);
            await _menuService.CreateAsync(createUpdateMenuDto);
            return Create("添加成功");
        }

        /// <summary>
        /// 更新菜单
        /// </summary>
        /// <param name="createUpdateMenuDto"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("edit")]
        [Description("更新菜单")]
        public async Task<ActionResult<object>> UpdateDept(
            [FromBody] CreateUpdateMenuDto createUpdateMenuDto)
        {
            RequiredHelper.IsValid(createUpdateMenuDto);
            await _menuService.UpdateAsync(createUpdateMenuDto);
            return NoContent("更新成功");
        }

        /// <summary>
        /// 删除菜单
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("delete")]
        [Description("删除菜单")]
        [NoJsonParamter]
        public async Task<ActionResult<object>> Delete([FromBody] HashSet<long> ids)
        {
            if (ids == null || ids.Count < 1)
            {
                return Error("ids is null");
            }

            await _menuService.DeleteAsync(ids);
            return Success("删除成功");
        }

        /// <summary>
        /// 构建树形菜单
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Description("构建树形菜单")]
        [Route("build")]
        [ApeVoloAuthorize(new[] {"admin", "menu:list"})]
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
        [Description("获取子菜单")]
        [Route("lazy")]
        [ApeVoloAuthorize(new[] {"admin", "menu:list"})]
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
        [Description("查看菜单列表")]
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
        [Description("导出菜单")]
        [Route("download")]
        public async Task<ActionResult<object>> Download(MenuQueryCriteria menuQueryCriteria)
        {
            var exportRowModels = await _menuService.DownloadAsync(menuQueryCriteria);

            var filepath = ExcelHelper.ExportData(exportRowModels, "菜单列表");

            var provider = new FileExtensionContentTypeProvider();
            FileInfo fileInfo = new FileInfo(filepath);
            var ext = fileInfo.Extension;
            new FileExtensionContentTypeProvider().Mappings.TryGetValue(ext, out var contently);
            return File(await System.IO.File.ReadAllBytesAsync(filepath), contently ?? "application/octet-stream",
                fileInfo.Name);
        }

        /// <summary>
        /// 获取同级与上级菜单
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost]
        [Description("获取同级与上级菜单")]
        [Route("superior")]
        [NoJsonParamter]
        [ApeVoloAuthorize(new[] {"admin", "menu:list"})]
        public async Task<ActionResult<object>> GetSuperior([FromBody] List<long> ids)
        {
            if (ids == null || ids.Count < 1)
            {
                return Error("ids is null");
            }

            var menuVos = await _menuService.FindSuperiorAsync(ids[0]);
            return menuVos.ToJsonByIgnore();
        }

        [HttpGet]
        [Description("获取所有子级菜单ID")]
        [Route("child")]
        [ApeVoloAuthorize(new[] {"admin", "menu:list"})]
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
}