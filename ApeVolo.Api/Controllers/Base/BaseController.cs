using System;
using ApeVolo.Api.ActionExtension.Json;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Model;
using ApeVolo.Common.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApeVolo.Api.Controllers.Base;

/// <summary>
/// 基控制器
/// </summary>
[JsonParamter]
public class BaseController : Controller
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="vm"></param>
    /// <returns></returns>
    private ContentResult JsonContent(ActionResultVm vm)
    {
        return new ContentResult
        {
            Content = new ActionResultVm
            {
                Status = vm.Status,
                ActionError = vm.ActionError,
                Message = vm.Message,
                Timestamp = DateTime.Now.ToUnixTimeStampMillisecond().ToString(),
                Path = Request.Path.Value?.ToLower() //HttpContext.Request.Path.Value?.ToLower()
            }.ToJson(),
            ContentType = "application/json; charset=utf-8",
            StatusCode = vm.Status
        };
    }


    /// <summary>
    /// 返回成功
    /// </summary>
    /// <param name="msg">消息</param>
    /// <returns></returns>
    protected ContentResult Success(string msg = "")
    {
        msg = msg.IsNullOrEmpty() ? Localized.Get("HttpOK") : msg;
        var vm = new ActionResultVm
        {
            Status = StatusCodes.Status200OK,
            Message = msg
        };

        return JsonContent(vm);
    }

    /// <summary>
    /// 创建成功
    /// </summary>
    /// <returns></returns>
    protected ContentResult Create(string msg = "")
    {
        msg = msg.IsNullOrEmpty() ? Localized.Get("HttpCreated") : msg;
        var vm = new ActionResultVm
        {
            Status = StatusCodes.Status201Created,
            Message = msg
        };

        return JsonContent(vm);
    }

    /// <summary>
    /// 更新成功 无需刷新
    /// </summary>
    /// <returns></returns>
    protected ContentResult NoContent(string msg = "")
    {
        return new ContentResult
        {
            ContentType = "application/json; charset=utf-8",
            StatusCode = StatusCodes.Status204NoContent
        };
    }

    /// <summary>
    /// 返回错误
    /// </summary>
    /// <param name="msg">错误提示</param>
    /// <returns></returns>
    protected ContentResult Error(string msg = "")
    {
        msg = msg.IsNullOrEmpty() ? Localized.Get("HttpBadRequest") : msg;
        var vm = new ActionResultVm
        {
            Status = StatusCodes.Status400BadRequest,
            Message = msg,
            ActionError = new ActionError()
        };

        return JsonContent(vm);
    }

    /// <summary>
    /// 返回错误
    /// </summary>
    /// <param name="actionError">错误集合</param>
    /// <returns></returns>
    protected ContentResult Error(ActionError actionError)
    {
        var vm = new ActionResultVm
        {
            Status = StatusCodes.Status400BadRequest,
            ActionError = actionError,
            //Message = Localized.Get("HttpBadRequest")
            Message = actionError.GetFirstError()
        };

        return JsonContent(vm);
    }
}
