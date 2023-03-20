using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using ApeVolo.Api.ActionExtension.Json;
using ApeVolo.Api.Controllers.Base;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Helper.Excel;
using ApeVolo.Common.Model;
using ApeVolo.Common.Resources;
using ApeVolo.IBusiness.Dto.System.AppSecret;
using ApeVolo.IBusiness.Interface.System.AppSecret;
using ApeVolo.IBusiness.QueryModel;
using ApeVolo.IBusiness.RequestModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace ApeVolo.Api.Controllers.System.AppSecret;

/// <summary>
/// 全局设置管理
/// </summary>
[Area("AppSecret")]
[Route("/api/appSecret")]
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
    /// 新增设置
    /// </summary>
    /// <param name="createUpdateAppSecretDto"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("create")]
    [Description("{0}Add")]
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
    /// 更新设置
    /// </summary>
    /// <param name="createUpdateAppSecretDto"></param>
    /// <returns></returns>
    [HttpPut]
    [Route("edit")]
    [Description("{0}Edit")]
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
    /// 删除设置
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

        await _appSecretService.DeleteAsync(idCollection.IdArray);
        return Success();
    }

    /// <summary>
    /// 获取设置列表
    /// </summary>
    /// <param name="appsecretQueryCriteria"></param>
    /// <param name="pagination"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("query")]
    [Description("{0}List")]
    public async Task<ActionResult<object>> Query(AppsecretQueryCriteria appsecretQueryCriteria,
        Pagination pagination)
    {
        var appSecretList = await _appSecretService.QueryAsync(appsecretQueryCriteria, pagination);

        return new ActionResultVm<AppSecretDto>
        {
            Content = appSecretList,
            TotalElements = pagination.TotalElements
        }.ToJson();
    }


    /// <summary>
    /// 导出设置
    /// </summary>
    /// <param name="appsecretQueryCriteria"></param>
    /// <returns></returns>
    [HttpGet]
    [Description("{0}Export")]
    [Route("download")]
    public async Task<ActionResult<object>> Download(AppsecretQueryCriteria appsecretQueryCriteria)
    {
        var exportRowModels = await _appSecretService.DownloadAsync(appsecretQueryCriteria);

        var filepath = ExcelHelper.ExportData(exportRowModels, Localized.Get("AppSecret"));

        FileInfo fileInfo = new FileInfo(filepath);
        var ext = fileInfo.Extension;
        new FileExtensionContentTypeProvider().Mappings.TryGetValue(ext, out var contently);
        return File(await global::System.IO.File.ReadAllBytesAsync(filepath), contently ?? "application/octet-stream",
            fileInfo.Name);
    }

    #endregion
}