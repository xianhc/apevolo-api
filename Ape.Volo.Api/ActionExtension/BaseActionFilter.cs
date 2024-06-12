using System;
using System.Threading.Tasks;
using Ape.Volo.Common.Extensions;
using Ape.Volo.Common.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Ape.Volo.Api.ActionExtension;

public class BaseActionFilter : Attribute, IAsyncActionFilter
{
    protected HttpContext HttpContext;

    public virtual async Task OnActionExecuting(ActionExecutingContext context)
    {
        await Task.CompletedTask;
    }

    public virtual async Task OnActionExecuted(ActionExecutedContext context)
    {
        await Task.CompletedTask;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        await OnActionExecuting(context);
        if (context.Result == null)
        {
            var nextContext = await next();
            await OnActionExecuted(nextContext);
        }
    }

    /// <summary>
    /// 返回JSON
    /// </summary>
    /// <param name="vm"></param>
    /// <returns></returns>
    public ContentResult JsonContent(ActionResultVm vm)
    {
        return new ContentResult
        {
            Content = new ActionResultVm
            {
                Status = vm.Status,
                Message = vm.Message,
                Timestamp = DateTime.Now.ToUnixTimeStampMillisecond().ToString(),
                Path = HttpContext.Request.Path.Value?.ToLower()
            }.ToJson(),
            ContentType = "application/json; charset=utf-8",
            StatusCode = vm.Status
        };
    }

    /// <summary>
    /// 返回错误
    /// </summary>
    /// <param name="msg">错误提示</param>
    /// <returns></returns>
    public ContentResult Error(string msg)
    {
        var vm = new ActionResultVm
        {
            Status = StatusCodes.Status400BadRequest,
            Message = msg
        };

        return JsonContent(vm);
    }

    /// <summary>
    /// 返回错误
    /// </summary>
    /// <param name="msg">错误提示</param>
    /// <param name="errorCode">错误代码</param>
    /// <returns></returns>
    public ContentResult Error(string msg, int errorCode)
    {
        var vm = new ActionResultVm
        {
            Status = errorCode,
            Message = msg
        };

        return JsonContent(vm);
    }
}
