using System.ComponentModel;
using System.Threading.Tasks;
using Ape.Volo.Api.Controllers.Base;
using Ape.Volo.Common.Extention;
using Ape.Volo.Common.Helper;
using Ape.Volo.Common.Model;
using Ape.Volo.IBusiness.Dto.System;
using Ape.Volo.IBusiness.Interface.System;
using Ape.Volo.IBusiness.QueryModel;
using Ape.Volo.IBusiness.RequestModel;
using Microsoft.AspNetCore.Mvc;

namespace Ape.Volo.Api.Controllers.System;

/// <summary>
/// 全局设置管理
/// </summary>
[Area("系统管理")]
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
    [Description("创建")]
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
    [Description("编辑")]
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

        await _settingService.DeleteAsync(idCollection.IdArray);
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
    [Description("查询")]
    public async Task<ActionResult<object>> Query(SettingQueryCriteria settingQueryCriteria, Pagination pagination)
    {
        var settingList = await _settingService.QueryAsync(settingQueryCriteria, pagination);

        return JsonContent(new ActionResultVm<SettingDto>
        {
            Content = settingList,
            TotalElements = pagination.TotalElements
        });
    }


    /// <summary>
    /// 导出设置
    /// </summary>
    /// <param name="settingQueryCriteria"></param>
    /// <returns></returns>
    [HttpGet]
    [Description("导出")]
    [Route("download")]
    public async Task<ActionResult<object>> Download(SettingQueryCriteria settingQueryCriteria)
    {
        var settingExports = await _settingService.DownloadAsync(settingQueryCriteria);
        var data = new ExcelHelper().GenerateExcel(settingExports, out var mimeType);
        return File(data, mimeType);
    }

    #endregion
}
