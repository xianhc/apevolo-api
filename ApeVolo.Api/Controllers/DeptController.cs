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
using ApeVolo.IBusiness.Dto.Core;
using ApeVolo.IBusiness.EditDto.Core;
using ApeVolo.IBusiness.Interface.Core;
using ApeVolo.IBusiness.QueryModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace ApeVolo.Api.Controllers;

/// <summary>
/// 部门管理
/// </summary>
[Area("dept")]
[Route("/api/dept")]
public class DeptController : BaseApiController
{
    #region 构造函数

    public DeptController(IDepartmentService departmentService, IRoleDeptService roleDeptService,
        IUserService userService)
    {
        _departmentService = departmentService;
        _roleDeptService = roleDeptService;
        _userService = userService;
    }

    #endregion

    #region 字段

    private readonly IDepartmentService _departmentService;
    private readonly IRoleDeptService _roleDeptService;
    private readonly IUserService _userService;

    #endregion

    #region 对内接口

    /// <summary>
    /// 新增部门
    /// </summary>
    /// <param name="createUpdateDepartmentDto"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("create")]
    [Description("新增部门")]
    public async Task<ActionResult<object>> Create(
        [FromBody] CreateUpdateDepartmentDto createUpdateDepartmentDto)
    {
        RequiredHelper.IsValid(createUpdateDepartmentDto);
        await _departmentService.CreateAsync(createUpdateDepartmentDto);
        return Create("添加成功");
    }


    /// <summary>
    /// 更新部门
    /// </summary>
    /// <param name="createUpdateDepartmentDto"></param>
    /// <returns></returns>
    [HttpPut]
    [Route("edit")]
    [Description("更新部门")]
    public async Task<ActionResult<object>> Update(
        [FromBody] CreateUpdateDepartmentDto createUpdateDepartmentDto)
    {
        RequiredHelper.IsValid(createUpdateDepartmentDto);
        await _departmentService.UpdateAsync(createUpdateDepartmentDto);
        return NoContent("更新成功");
    }


    /// <summary>
    /// 删除部门
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route("delete")]
    [Description("删除部门")]
    [NoJsonParamter]
    public async Task<ActionResult<object>> Delete([FromBody] HashSet<long> ids)
    {
        if (ids == null || ids.Count < 1)
            return Error("ids is null");

        var deptIds = new List<long>();
        ids.ForEach(async id =>
        {
            deptIds.Add(id);
            var deptDtolist = await _departmentService.QueryByPIdAsync(id);
            deptDtolist.ForEach(deptDto =>
            {
                if (!deptIds.Contains(deptDto.Id)) deptIds.Add(deptDto.Id);
            });
        });
        var depts = await _roleDeptService.QueryByDeptIdsAsync(deptIds);
        if (depts.Count > 0) return Error("所选部门存在角色关联，请解除后再试！");

        var users = await _userService.QueryByDeptIdsAsync(deptIds);
        if (users.Count > 0) return Error("所选部门存在用户关联，请解除后再试！");

        await _departmentService.DeleteAsync(ids);
        return Success();
    }


    /// <summary>
    /// 查看部门列表
    /// </summary>
    /// <param name="deptQueryCriteria"></param>
    /// <param name="pagination"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("query")]
    [Description("查看部门列表")]
    public async Task<ActionResult<object>> Query(DeptQueryCriteria deptQueryCriteria,
        Pagination pagination)
    {
        var deptList = await _departmentService.QueryAsync(deptQueryCriteria, pagination);


        return new ActionResultVm<DepartmentDto>
        {
            Content = deptList,
            TotalElements = pagination.TotalElements
        }.ToJson();
    }

    /// <summary>
    /// 导出部门
    /// </summary>
    /// <param name="deptQueryCriteria"></param>
    /// <returns></returns>
    [HttpGet]
    [Description("导出部门")]
    [Route("download")]
    public async Task<ActionResult<object>> Download(DeptQueryCriteria deptQueryCriteria)
    {
        var exportRowModels = await _departmentService.DownloadAsync(deptQueryCriteria);

        var filepath = ExcelHelper.ExportData(exportRowModels, "部门列表");

        var provider = new FileExtensionContentTypeProvider();
        var fileInfo = new FileInfo(filepath);
        var ext = fileInfo.Extension;
        new FileExtensionContentTypeProvider().Mappings.TryGetValue(ext, out var contently);
        return File(await System.IO.File.ReadAllBytesAsync(filepath), contently ?? "application/octet-stream",
            fileInfo.Name);
    }


    /// <summary>
    /// 获取同级与父级部门
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("superior")]
    [Description("获取同级、父级部门")]
    [ApeVoloAuthorize(new[] { "admin", "dept:list" })]
    public async Task<ActionResult<object>> GetSuperior([FromBody] List<long> ids)
    {
        if (ids == null || ids.Count < 1) return Error("ids is null");

        var deptList = await _departmentService.QuerySuperiorDeptAsync(ids);

        return new ActionResultVm<DepartmentDto>
        {
            Content = deptList,
            TotalElements = deptList.Count
        }.ToJson();
    }

    #endregion
}