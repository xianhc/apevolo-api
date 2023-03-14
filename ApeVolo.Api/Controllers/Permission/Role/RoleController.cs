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
using ApeVolo.IBusiness.Dto.Permission.Role;
using ApeVolo.IBusiness.Interface.Permission.Role;
using ApeVolo.IBusiness.Interface.Permission.User;
using ApeVolo.IBusiness.QueryModel;
using ApeVolo.IBusiness.RequestModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace ApeVolo.Api.Controllers.Permission.Role;

/// <summary>
/// 角色管理
/// </summary>
[Area("Role")]
[Route("/api/role")]
public class RoleController : BaseApiController
{
    #region 字段

    private readonly IRoleService _roleService;
    private readonly IUserRolesService _userRolesService;

    #endregion

    #region 构造函数

    public RoleController(IRoleService roleService, IUserRolesService userRolesService)
    {
        _roleService = roleService;
        _userRolesService = userRolesService;
    }

    #endregion

    #region 内部接口

    /// <summary>
    /// 添加角色
    /// </summary>
    /// <param name="createUpdateRoleDto"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("create")]
    [Description("{0}Add")]
    public async Task<ActionResult<object>> Create([FromBody] CreateUpdateRoleDto createUpdateRoleDto)
    {
        if (!ModelState.IsValid)
        {
            var actionError = ModelState.GetErrors();
            return Error(actionError);
        }

        await _roleService.CreateAsync(createUpdateRoleDto);
        return Create();
    }

    /// <summary>
    /// 更新角色
    /// </summary>
    /// <param name="createUpdateRoleDto"></param>
    /// <returns></returns>
    [HttpPut]
    [Description("{0}Edit")]
    [Route("edit")]
    public async Task<ActionResult<object>> Update([FromBody] CreateUpdateRoleDto createUpdateRoleDto)
    {
        if (!ModelState.IsValid)
        {
            var actionError = ModelState.GetErrors();
            return Error(actionError);
        }

        await _roleService.UpdateAsync(createUpdateRoleDto);
        return NoContent();
    }

    /// <summary>
    /// 删除角色
    /// </summary>
    /// <param name="collection"></param>
    /// <returns></returns>
    [HttpDelete]
    [Description("{0}Delete")]
    [Route("delete")]
    [NoJsonParamter]
    public async Task<ActionResult<object>> Delete([FromBody] IdCollection collection)
    {
        if (!ModelState.IsValid)
        {
            var actionError = ModelState.GetErrors();
            return Error(actionError);
        }

        //检查待删除的角色是否有用户存在
        var userRoles = await _userRolesService.QueryByRoleIdsAsync(collection.IdArray);
        if (!userRoles.IsNullOrEmpty() && userRoles.Count > 0)
        {
            return Error(Localized.Get("DataCannotDelete"));
        }

        await _roleService.DeleteAsync(collection.IdArray);
        return Success();
    }

    /// <summary>
    /// 查看单一角色
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("{id}")]
    [Description("ViewRole")]
    [ApeVoloAuthorize(new[] { "roles_list" })]
    public async Task<ActionResult<object>> QuerySingle(string id)
    {
        var role = await _roleService.QuerySingleAsync(id);
        return role.ToJson();
    }

    /// <summary>
    /// 查看角色列表
    /// </summary>
    /// <param name="roleQueryCriteria"></param>
    /// <param name="pagination"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("query")]
    [Description("{0}List")]
    public async Task<ActionResult<object>> Query(RoleQueryCriteria roleQueryCriteria,
        Pagination pagination)
    {
        var roleList = await _roleService.QueryAsync(roleQueryCriteria, pagination);

        return new ActionResultVm<RoleDto>
        {
            Content = roleList,
            TotalElements = pagination.TotalElements
        }.ToJson();
    }

    /// <summary>
    /// 导出角色列表
    /// </summary>
    /// <param name="roleQueryCriteria"></param>
    /// <returns></returns>
    [HttpGet]
    [Description("{0}Export")]
    [Route("download")]
    public async Task<ActionResult<object>> Download(RoleQueryCriteria roleQueryCriteria)
    {
        var exportRowModels = await _roleService.DownloadAsync(roleQueryCriteria);

        var filepath = ExcelHelper.ExportData(exportRowModels, Localized.Get("Role"));

        FileInfo fileInfo = new FileInfo(filepath);
        var ext = fileInfo.Extension;
        new FileExtensionContentTypeProvider().Mappings.TryGetValue(ext, out var contently);
        return File(await global::System.IO.File.ReadAllBytesAsync(filepath), contently ?? "application/octet-stream",
            fileInfo.Name);
    }

    /// <summary>
    /// 获取全部角色
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("all")]
    [Description("{0}List")]
    [ApeVoloAuthorize(new[] { "admin", "roles_list" })]
    public async Task<ActionResult<object>> GetAllRoles()
    {
        var allRoles = await _roleService.QueryAllAsync();

        return allRoles.ToJson();
    }

    /// <summary>
    /// 获取当前用户角色等级
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("level")]
    [Description("UserRoleLevel")]
    [ApeVoloAuthorize(new[] { "admin", "roles_list" })]
    public async Task<ActionResult<object>> GetRoleLevel(int? level)
    {
        var curLevel = await _roleService.VerificationUserRoleLevelAsync(level);

        Dictionary<string, int> keyValuePairs = new Dictionary<string, int> { { "level", curLevel } };
        return keyValuePairs.ToJson();
    }

    #endregion
}