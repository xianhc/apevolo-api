using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using ApeVolo.Api.Controllers.Base;
using ApeVolo.Common.AttributeExt;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Helper;
using ApeVolo.Common.Model;
using ApeVolo.Common.Resources;
using ApeVolo.IBusiness.Dto.Permission.Role;
using ApeVolo.IBusiness.Interface.Permission.Role;
using ApeVolo.IBusiness.Interface.Permission.User;
using ApeVolo.IBusiness.QueryModel;
using ApeVolo.IBusiness.RequestModel;
using Microsoft.AspNetCore.Mvc;

namespace ApeVolo.Api.Controllers.Permission.Role;

/// <summary>
/// 角色管理
/// </summary>
[Area("Permission")]
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
    [Description("Add")]
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
    [Description("Edit")]
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
    /// <param name="idCollection"></param>
    /// <returns></returns>
    [HttpDelete]
    [Description("Delete")]
    [Route("delete")]
    public async Task<ActionResult<object>> Delete([FromBody] IdCollection idCollection)
    {
        if (!ModelState.IsValid)
        {
            var actionError = ModelState.GetErrors();
            return Error(actionError);
        }

        //检查待删除的角色是否有用户存在
        var userRoles = await _userRolesService.QueryByRoleIdsAsync(idCollection.IdArray);
        if (!userRoles.IsNullOrEmpty() && userRoles.Count > 0)
        {
            return Error(Localized.Get("DataCannotDelete"));
        }

        await _roleService.DeleteAsync(idCollection.IdArray);
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
    [Description("List")]
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
    [Description("Export")]
    [Route("download")]
    public async Task<ActionResult<object>> Download(RoleQueryCriteria roleQueryCriteria)
    {
        var roleExports = await _roleService.DownloadAsync(roleQueryCriteria);
        var data = new ExcelHelper().GenerateExcel(roleExports, out var mimeType);
        return File(data, mimeType);
    }

    /// <summary>
    /// 获取全部角色
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("all")]
    [Description("List")]
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