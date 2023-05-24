using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using ApeVolo.Api.Controllers.Base;
using ApeVolo.Common.AttributeExt;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Helper.Excel;
using ApeVolo.Common.Model;
using ApeVolo.Common.Resources;
using ApeVolo.IBusiness.Dto.Permission.Department;
using ApeVolo.IBusiness.Interface.Permission.Department;
using ApeVolo.IBusiness.Interface.Permission.Role;
using ApeVolo.IBusiness.Interface.Permission.User;
using ApeVolo.IBusiness.QueryModel;
using ApeVolo.IBusiness.RequestModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace ApeVolo.Api.Controllers.Permission.Department;

/// <summary>
/// 部门管理
/// </summary>
[Area("Permission")]
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
    [Description("{0}Add")]
    public async Task<ActionResult<object>> Create(
        [FromBody] CreateUpdateDepartmentDto createUpdateDepartmentDto)
    {
        if (!ModelState.IsValid)
        {
            var actionError = ModelState.GetErrors();
            return Error(actionError);
        }

        await _departmentService.CreateAsync(createUpdateDepartmentDto);
        return Create();
    }


    /// <summary>
    /// 更新部门
    /// </summary>
    /// <param name="createUpdateDepartmentDto"></param>
    /// <returns></returns>
    [HttpPut]
    [Route("edit")]
    [Description("{0}Edit")]
    public async Task<ActionResult<object>> Update(
        [FromBody] CreateUpdateDepartmentDto createUpdateDepartmentDto)
    {
        if (!ModelState.IsValid)
        {
            var actionError = ModelState.GetErrors();
            return Error(actionError);
        }

        await _departmentService.UpdateAsync(createUpdateDepartmentDto);
        return NoContent();
    }


    /// <summary>
    /// 删除部门
    /// </summary>
    /// <param name="idCollection"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route("delete")]
    [Description("{0}Delete")]
    public async Task<ActionResult<object>> Delete([FromBody] IdCollection idCollection)
    {
        if (!ModelState.IsValid)
        {
            var actionError = ModelState.GetErrors();
            return Error(actionError);
        }

        var deptIds = new List<long>();
        foreach (var id in idCollection.IdArray)
        {
            deptIds.Add(id);
            var deptDtolist = await _departmentService.QueryByPIdAsync(id);
            deptDtolist.ForEach(deptDto =>
            {
                if (!deptIds.Contains(deptDto.Id)) deptIds.Add(deptDto.Id);
            });
        }

        var depts = await _roleDeptService.QueryByDeptIdsAsync(deptIds);
        if (depts.Count > 0) return Error("所选部门存在角色关联，请解除后再试！");

        var users = await _userService.QueryByDeptIdsAsync(deptIds);
        if (users.Count > 0) return Error("所选部门存在用户关联，请解除后再试！");

        await _departmentService.DeleteAsync(idCollection.IdArray);
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
    [Description("{0}List")]
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
    [Description("{0}Export")]
    [Route("download")]
    public async Task<ActionResult<object>> Download(DeptQueryCriteria deptQueryCriteria)
    {
        var exportRowModels = await _departmentService.DownloadAsync(deptQueryCriteria);

        var filepath = ExcelHelper.ExportData(exportRowModels, Localized.Get("Department"));

        var fileInfo = new FileInfo(filepath);
        var ext = fileInfo.Extension;
        new FileExtensionContentTypeProvider().Mappings.TryGetValue(ext, out var contently);
        return File(await global::System.IO.File.ReadAllBytesAsync(filepath), contently ?? "application/octet-stream",
            fileInfo.Name);
    }


    /// <summary>
    /// 获取同级与父级部门
    /// </summary>
    /// <param name="idCollection"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("superior")]
    [Description("SiblingParentDepartment")]
    [ApeVoloAuthorize(new[] { "admin", "dept_list" })]
    public async Task<ActionResult<object>> GetSuperior([FromBody] IdCollection idCollection)
    {
        if (!ModelState.IsValid)
        {
            var actionError = ModelState.GetErrors();
            return Error(actionError);
        }

        var deptList = await _departmentService.QuerySuperiorDeptAsync(idCollection.IdArray);

        return new ActionResultVm<DepartmentDto>
        {
            Content = deptList,
            TotalElements = deptList.Count
        }.ToJson();
    }

    #endregion
}