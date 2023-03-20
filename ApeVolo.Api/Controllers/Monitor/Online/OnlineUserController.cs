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
using ApeVolo.IBusiness.Interface.Monitor.Online;
using ApeVolo.IBusiness.RequestModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace ApeVolo.Api.Controllers.Monitor.Online;

/// <summary>
/// 在线用户
/// </summary>
[Area("OnlineUser")]
[Route("/api/online")]
public class OnlineUserController : BaseApiController
{
    private readonly IOnlineUserService _onlineUserService;

    public OnlineUserController(IOnlineUserService onlineUserService)
    {
        _onlineUserService = onlineUserService;
    }

    #region 对内接口

    /// <summary>
    /// 在线用户列表
    /// </summary>
    /// <param name="pagination"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("query")]
    [Description("{0}List")]
    public async Task<ActionResult<object>> Query(Pagination pagination)
    {
        var onlineUserList = await _onlineUserService.QueryAsync(pagination);

        return new ActionResultVm<Common.WebApp.OnlineUser>
        {
            Content = onlineUserList,
            TotalElements = onlineUserList.Count
        }.ToJson();
    }

    /// <summary>
    /// 强制登出用户
    /// </summary>
    /// <param name="idCollection"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route("out")]
    [Description("OutOnlineUser")]
    [ApeVoloAuthorize(new[] { "admin" })]
    public async Task<ActionResult<object>> DropOut([FromBody] IdCollectionString idCollection)
    {
        if (!ModelState.IsValid)
        {
            var actionError = ModelState.GetErrors();
            return Error(actionError);
        }

        await _onlineUserService.DropOutAsync(idCollection.IdArray);

        return Success();
    }

    /// <summary>
    /// 导出在线用户
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Description("{0}Export")]
    [Route("download")]
    [ApeVoloAuthorize(new[] { "admin" })]
    public async Task<ActionResult<object>> Download()
    {
        var exportRowModels = await _onlineUserService.DownloadAsync();

        var filepath = ExcelHelper.ExportData(exportRowModels, Localized.Get("OnlineUser"));

        FileInfo fileInfo = new FileInfo(filepath);
        var ext = fileInfo.Extension;
        new FileExtensionContentTypeProvider().Mappings.TryGetValue(ext, out var contently);
        return File(await global::System.IO.File.ReadAllBytesAsync(filepath), contently ?? "application/octet-stream",
            fileInfo.Name);
    }

    #endregion
}