using System.ComponentModel;
using System.Threading.Tasks;
using Ape.Volo.Api.Controllers.Base;
using Ape.Volo.Common.Extensions;
using Ape.Volo.Common.Helper;
using Ape.Volo.Common.Model;
using Ape.Volo.IBusiness.Dto.System;
using Ape.Volo.IBusiness.Interface.System;
using Ape.Volo.IBusiness.QueryModel;
using Ape.Volo.IBusiness.RequestModel;
using Microsoft.AspNetCore.Mvc;

namespace Ape.Volo.Api.Controllers.System;

/// <summary>
/// 租户管理
/// </summary>
[Area("租户管理")]
[Route("/api/tenant", Order = 19)]
public class TenantController : BaseApiController
{
    #region 字段

    private readonly ITenantService _tenantService;

    #endregion

    #region 构造函数

    public TenantController(ITenantService tenantService)
    {
        _tenantService = tenantService;
    }

    #endregion

    #region 内部接口

    /// <summary>
    /// 新增租户
    /// </summary>
    /// <param name="createUpdateTenantDto"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("create")]
    [Description("创建")]
    public async Task<ActionResult> Create(
        [FromBody] CreateUpdateTenantDto createUpdateTenantDto)
    {
        if (!ModelState.IsValid)
        {
            var actionError = ModelState.GetErrors();
            return Error(actionError);
        }

        await _tenantService.CreateAsync(createUpdateTenantDto);
        return Create();
    }

    /// <summary>
    /// 更新租户
    /// </summary>
    /// <param name="createUpdateTenantDto"></param>
    /// <returns></returns>
    [HttpPut]
    [Route("edit")]
    [Description("编辑")]
    public async Task<ActionResult> Update(
        [FromBody] CreateUpdateTenantDto createUpdateTenantDto)
    {
        if (!ModelState.IsValid)
        {
            var actionError = ModelState.GetErrors();
            return Error(actionError);
        }

        await _tenantService.UpdateAsync(createUpdateTenantDto);
        return NoContent();
    }

    /// <summary>
    /// 删除租户
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

        await _tenantService.DeleteAsync(idCollection.IdArray);
        return Success();
    }

    /// <summary>
    /// 获取租户列表
    /// </summary>
    /// <param name="tenantQueryCriteria"></param>
    /// <param name="pagination"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("query")]
    [Description("查询")]
    public async Task<ActionResult> Query(TenantQueryCriteria tenantQueryCriteria, Pagination pagination)
    {
        var tenantList = await _tenantService.QueryAsync(tenantQueryCriteria, pagination);

        return JsonContent(new ActionResultVm<TenantDto>
        {
            Content = tenantList,
            TotalElements = pagination.TotalElements
        });
    }

    /// <summary>
    /// 获取所有租户
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("queryAll")]
    [Description("查询全部")]
    public async Task<ActionResult> QueryAll()
    {
        var tenantList = await _tenantService.QueryAllAsync();

        return JsonContent(new ActionResultVm<TenantDto>
        {
            Content = tenantList,
            TotalElements = tenantList.Count
        });
    }


    /// <summary>
    /// 导出租户
    /// </summary>
    /// <param name="tenantQueryCriteria"></param>
    /// <returns></returns>
    [HttpGet]
    [Description("导出")]
    [Route("download")]
    public async Task<ActionResult> Download(TenantQueryCriteria tenantQueryCriteria)
    {
        var tenantExports = await _tenantService.DownloadAsync(tenantQueryCriteria);
        var data = new ExcelHelper().GenerateExcel(tenantExports, out var mimeType, out var fileName);
        return new FileContentResult(data, mimeType)
        {
            FileDownloadName = fileName
        };
    }

    #endregion
}
