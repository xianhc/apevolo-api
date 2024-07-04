using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Ape.Volo.Api.Controllers.Base;
using Ape.Volo.Common.Extensions;
using Ape.Volo.Common.Helper;
using Ape.Volo.Common.Model;
using Ape.Volo.IBusiness.Dto.Permission;
using Ape.Volo.IBusiness.Interface.Permission;
using Ape.Volo.IBusiness.QueryModel;
using Ape.Volo.IBusiness.RequestModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ape.Volo.Api.Controllers.Permission;

/// <summary>
/// 部门管理
/// </summary>
[Area("部门管理")]
[Route("/api/dept", Order = 4)]
public class DeptController : BaseApiController
{
    #region 构造函数

    public DeptController(IDepartmentService departmentService)
    {
        _departmentService = departmentService;
    }

    #endregion

    #region 字段

    private readonly IDepartmentService _departmentService;

    #endregion

    #region 对内接口

    /// <summary>
    /// 新增部门
    /// </summary>
    /// <param name="createUpdateDepartmentDto"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("create")]
    [Description("创建")]
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
    [Description("编辑")]
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
    [Description("删除")]
    public async Task<ActionResult<object>> Delete([FromBody] IdCollection idCollection)
    {
        if (!ModelState.IsValid)
        {
            var actionError = ModelState.GetErrors();
            return Error(actionError);
        }

        List<long> ids = new List<long>(idCollection.IdArray);
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
    [Description("查询")]
    public async Task<ActionResult<object>> Query(DeptQueryCriteria deptQueryCriteria,
        Pagination pagination)
    {
        var deptList = await _departmentService.QueryAsync(deptQueryCriteria, pagination);


        return JsonContent(new ActionResultVm<DepartmentDto>
        {
            Content = deptList,
            TotalElements = pagination.TotalElements
        });
    }


    [HttpGet]
    [Route("queryTree")]
    [Description("树形部门数据")]
    public async Task<ActionResult<object>> QueryTree()
    {
        var deptList = await _departmentService.QueryAllAsync();

        var menuTree = TreeHelper<DepartmentDto>.ListToTrees(deptList, "Id", "ParentId", 0);
        return menuTree.ToJson();
    }


    /// <summary>
    /// 导出部门
    /// </summary>
    /// <param name="deptQueryCriteria"></param>
    /// <returns></returns>
    [HttpGet]
    [Description("导出")]
    [Route("download")]
    public async Task<ActionResult<object>> Download(DeptQueryCriteria deptQueryCriteria)
    {
        var deptExports = await _departmentService.DownloadAsync(deptQueryCriteria);
        var data = new ExcelHelper().GenerateExcel(deptExports, out var mimeType);
        return File(data, mimeType);
    }


    /// <summary>
    /// 获取同级与父级部门
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("superior")]
    [Description("获取同级、父级部门")]
    public async Task<ActionResult<object>> GetSuperior(long id)
    {
        if (id.IsNullOrEmpty())
        {
            return Error("id cannot be empty");
        }

        var deptList = await _departmentService.QuerySuperiorDeptAsync(id);

        return JsonContent(new ActionResultVm<DepartmentDto>
        {
            Content = deptList,
            TotalElements = deptList.Count
        });
    }

    #endregion
}
