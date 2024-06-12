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
/// 应用密钥管理
/// </summary>
[Area("应用密钥管理")]
[Route("/api/appSecret", Order = 11)]
public class AppSecretController : BaseApiController
{
    #region 字段

    private readonly IAppSecretService _appSecretService;

    #endregion

    #region 构造函数

    public AppSecretController(IAppSecretService appSecretService)
    {
        _appSecretService = appSecretService;
    }

    #endregion

    #region 内部接口

    /// <summary>
    /// 新增应用密钥
    /// </summary>
    /// <param name="createUpdateAppSecretDto"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("create")]
    [Description("创建")]
    public async Task<ActionResult<object>> Create(
        [FromBody] CreateUpdateAppSecretDto createUpdateAppSecretDto)
    {
        if (!ModelState.IsValid)
        {
            var actionError = ModelState.GetErrors();
            return Error(actionError);
        }

        await _appSecretService.CreateAsync(createUpdateAppSecretDto);
        return Create();
    }

    /// <summary>
    /// 更新应用密钥
    /// </summary>
    /// <param name="createUpdateAppSecretDto"></param>
    /// <returns></returns>
    [HttpPut]
    [Route("edit")]
    [Description("编辑")]
    public async Task<ActionResult<object>> Update(
        [FromBody] CreateUpdateAppSecretDto createUpdateAppSecretDto)
    {
        if (!ModelState.IsValid)
        {
            var actionError = ModelState.GetErrors();
            return Error(actionError);
        }

        await _appSecretService.UpdateAsync(createUpdateAppSecretDto);
        return NoContent();
    }

    /// <summary>
    /// 删除应用密钥
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

        await _appSecretService.DeleteAsync(idCollection.IdArray);
        return Success();
    }

    /// <summary>
    /// 获取应用密钥列表
    /// </summary>
    /// <param name="appsecretQueryCriteria"></param>
    /// <param name="pagination"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("query")]
    [Description("查询")]
    public async Task<ActionResult<object>> Query(AppsecretQueryCriteria appsecretQueryCriteria,
        Pagination pagination)
    {
        var appSecretList = await _appSecretService.QueryAsync(appsecretQueryCriteria, pagination);

        return JsonContent(new ActionResultVm<AppSecretDto>
        {
            Content = appSecretList,
            TotalElements = pagination.TotalElements
        });
    }


    /// <summary>
    /// 导出应用密钥
    /// </summary>
    /// <param name="appsecretQueryCriteria"></param>
    /// <returns></returns>
    [HttpGet]
    [Description("导出")]
    [Route("download")]
    public async Task<ActionResult<object>> Download(AppsecretQueryCriteria appsecretQueryCriteria)
    {
        var appSecretExports = await _appSecretService.DownloadAsync(appsecretQueryCriteria);
        var data = new ExcelHelper().GenerateExcel(appSecretExports, out var mimeType);
        return File(data, mimeType);
    }

    #endregion
}
