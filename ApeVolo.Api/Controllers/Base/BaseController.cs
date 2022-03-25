using System.Text;
using ApeVolo.Api.ActionExtension.Json;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Model;
using ApeVolo.Common.WebApp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApeVolo.Api.Controllers.Base;

/// <summary>
/// 基控制器
/// </summary>
[JsonParamter]
public class BaseController : ControllerBase
{
    /// <summary>
    /// 返回JSON
    /// </summary>
    /// <param name="jsonStr">json字符串</param>
    /// <returns></returns>
    private ContentResult JsonContent(string jsonStr)
    {
        return base.Content(jsonStr, "application/json", Encoding.UTF8);
    }

    private ContentResult JsonContent(ActionResultVm vm)
    {
        return new()
        {
            Content = new ActionResultVm
            {
                Status = vm.Status,
                Error = vm.Error,
                Message = vm.Message,
                Path = HttpContextCore.CurrentHttpContext.Request.Path.Value?.ToLower()
            }.ToJson(),
            ContentType = "application/json; charset=utf-8",
            StatusCode = vm.Status
        };
    }

    /// <summary>
    /// 返回html
    /// </summary>
    /// <param name="body">html内容</param>
    /// <returns></returns>
    protected ContentResult HtmlContent(string body)
    {
        return base.Content(body);
    }


    /// <summary>
    /// 返回成功
    /// </summary>
    /// <param name="msg">消息</param>
    /// <returns></returns>
    protected ContentResult Success(string msg = "请求成功！")
    {
        var res = new ActionResultVm
        {
            Status = StatusCodes.Status200OK,
            Message = msg,
            Path = HttpContextCore.CurrentHttpContext.Request.Path.Value?.ToLower()
        };

        return JsonContent(res.ToJson());
    }

    /// <summary>
    /// 创建成功
    /// </summary>
    /// <returns></returns>
    protected ContentResult Create(string msg = "创建成功！")
    {
        var vm = new ActionResultVm
        {
            Status = StatusCodes.Status201Created,
            Message = msg,
            Path = HttpContextCore.CurrentHttpContext.Request.Path.Value?.ToLower()
        };

        return JsonContent(vm);
    }

    /// <summary>
    /// 更新成功 无需刷新
    /// </summary>
    /// <returns></returns>
    protected ContentResult NoContent(string msg = "编辑成功！")
    {
        var vm = new ActionResultVm
        {
            Status = StatusCodes.Status204NoContent,
            Message = msg,
            Path = HttpContextCore.CurrentHttpContext.Request.Path.Value?.ToLower()
        };

        return JsonContent(vm);
    }

    /// <summary>
    /// 返回错误
    /// </summary>
    /// <param name="msg">错误提示</param>
    /// <returns></returns>
    protected ContentResult Error(string msg = "请求失败！")
    {
        var vm = new ActionResultVm
        {
            Status = StatusCodes.Status400BadRequest,
            Error = "BadRequest",
            Message = msg,
            Path = HttpContextCore.CurrentHttpContext.Request.Path.Value?.ToLower()
        };

        return JsonContent(vm);
    }
}