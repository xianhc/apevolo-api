using System;
using System.Collections.Generic;
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
/// 角色管理
/// </summary>
[Area("角色管理")]
[Route("/api/role", Order = 2)]
public class RoleController : BaseApiController
{
    #region 字段

    private readonly IRoleService _roleService;

    #endregion

    #region 构造函数

    /// <summary>
    /// 
    /// </summary>
    /// <param name="roleService"></param>
    public RoleController(IRoleService roleService)
    {
        _roleService = roleService;
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
    [Description("创建")]
    public async Task<ActionResult> Create([FromBody] CreateUpdateRoleDto createUpdateRoleDto)
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
    [Description("编辑")]
    [Route("edit")]
    public async Task<ActionResult> Update([FromBody] CreateUpdateRoleDto createUpdateRoleDto)
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
    [Description("删除")]
    [Route("delete")]
    public async Task<ActionResult> Delete([FromBody] IdCollection idCollection)
    {
        if (!ModelState.IsValid)
        {
            var actionError = ModelState.GetErrors();
            return Error(actionError);
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
    [Route("querySingle")]
    [Description("查看指定角色")]
    public async Task<ActionResult> QuerySingle(string id)
    {
        if (id.IsNullOrEmpty())
        {
            return Error("id cannot be empty");
        }

        var newId = Convert.ToInt64(id);
        var role = await _roleService.TableWhere(x => x.Id == newId).Includes(x => x.MenuList).Includes(x => x.Apis)
            .Includes(x => x.DepartmentList).SingleAsync();
        return Ok(App.Mapper.MapTo<RoleDto>(role));
    }

    /// <summary>
    /// 查看角色列表
    /// </summary>
    /// <param name="roleQueryCriteria"></param>
    /// <param name="pagination"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("query")]
    [Description("查询")]
    public async Task<ActionResult> Query(RoleQueryCriteria roleQueryCriteria,
        Pagination pagination)
    {
        var roleList = await _roleService.QueryAsync(roleQueryCriteria, pagination);

        return JsonContent(new ActionResultVm<RoleDto>
        {
            Content = roleList,
            TotalElements = pagination.TotalElements
        });
    }

    /// <summary>
    /// 导出角色列表
    /// </summary>
    /// <param name="roleQueryCriteria"></param>
    /// <returns></returns>
    [HttpGet]
    [Description("导出")]
    [Route("download")]
    public async Task<ActionResult> Download(RoleQueryCriteria roleQueryCriteria)
    {
        var roleExports = await _roleService.DownloadAsync(roleQueryCriteria);
        var data = new ExcelHelper().GenerateExcel(roleExports, out var mimeType, out var fileName);
        return new FileContentResult(data, mimeType)
        {
            FileDownloadName = fileName
        };
    }

    /// <summary>
    /// 获取全部角色
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("queryAll")]
    [Description("查询全部")]
    public async Task<ActionResult> QueryAll()
    {
        var allRoles = await _roleService.QueryAllAsync();

        return Ok(allRoles);
    }

    /// <summary>
    /// 获取当前用户角色等级
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("level")]
    [Description("当前用户等级")]
    public async Task<ActionResult> GetRoleLevel(int? level)
    {
        var curLevel = await _roleService.VerificationUserRoleLevelAsync(level);

        var response = new
        {
            level = curLevel
        };

        return Ok(response);
    }

    #endregion
}
