using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using ApeVolo.Api.ActionExtension.Json;
using ApeVolo.Api.Controllers.Base;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Helper.Excel;
using ApeVolo.Common.Model;
using ApeVolo.Common.Resources;
using ApeVolo.IBusiness.Dto.System.Setting;
using ApeVolo.IBusiness.Interface.System.Setting;
using ApeVolo.IBusiness.QueryModel;
using ApeVolo.IBusiness.RequestModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace ApeVolo.Api.Controllers.System.Setting;

/// <summary>
/// 全局设置管理
/// </summary>
[Area("Setting")]
[Route("/api/setting")]
public class SettingController : BaseApiController
{
    #region 字段

    private readonly ISettingService _settingService;

    #endregion

    #region 构造函数

    public SettingController(ISettingService settingService)
    {
        _settingService = settingService;
    }

    #endregion

    #region 内部接口

    /// <summary>
    /// 新增设置
    /// </summary>
    /// <param name="createUpdateSettingDto"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("create")]
    [Description("{0}Add")]
    public async Task<ActionResult<object>> Create(
        [FromBody] CreateUpdateSettingDto createUpdateSettingDto)
    {
        if (!ModelState.IsValid)
        {
            var actionError = ModelState.GetErrors();
            return Error(actionError);
        }

        await _settingService.CreateAsync(createUpdateSettingDto);
        return Create();
    }

    /// <summary>
    /// 更新设置
    /// </summary>
    /// <param name="createUpdateSettingDto"></param>
    /// <returns></returns>
    [HttpPut]
    [Route("edit")]
    [Description("{0}Edit")]
    public async Task<ActionResult<object>> Update(
        [FromBody] CreateUpdateSettingDto createUpdateSettingDto)
    {
        if (!ModelState.IsValid)
        {
            var actionError = ModelState.GetErrors();
            return Error(actionError);
        }

        await _settingService.UpdateAsync(createUpdateSettingDto);
        return NoContent();
    }

    /// <summary>
    /// 删除设置
    /// </summary>
    /// <param name="collection"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route("delete")]
    [Description("{0}Delete")]
    [NoJsonParamter]
    public async Task<ActionResult<object>> Delete([FromBody] IdCollection collection)
    {
        if (!ModelState.IsValid)
        {
            var actionError = ModelState.GetErrors();
            return Error(actionError);
        }

        await _settingService.DeleteAsync(collection.IdArray);
        return Success();
    }

    /// <summary>
    /// 获取设置列表
    /// </summary>
    /// <param name="settingQueryCriteria"></param>
    /// <param name="pagination"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("query")]
    [Description("{0}List")]
    public async Task<ActionResult<object>> Query(SettingQueryCriteria settingQueryCriteria, Pagination pagination)
    {
        var settingList = await _settingService.QueryAsync(settingQueryCriteria, pagination);

        return new ActionResultVm<SettingDto>
        {
            Content = settingList,
            TotalElements = pagination.TotalElements
        }.ToJson();
    }


    /// <summary>
    /// 导出设置
    /// </summary>
    /// <param name="settingQueryCriteria"></param>
    /// <returns></returns>
    [HttpGet]
    [Description("{0}Export")]
    [Route("download")]
    public async Task<ActionResult<object>> Download(SettingQueryCriteria settingQueryCriteria)
    {
        var exportRowModels = await _settingService.DownloadAsync(settingQueryCriteria);

        var filepath = ExcelHelper.ExportData(exportRowModels, Localized.Get("Setting"));

        FileInfo fileInfo = new FileInfo(filepath);
        var ext = fileInfo.Extension;
        new FileExtensionContentTypeProvider().Mappings.TryGetValue(ext, out var contently);
        return File(await global::System.IO.File.ReadAllBytesAsync(filepath), contently ?? "application/octet-stream",
            fileInfo.Name);
    }

    #endregion
}