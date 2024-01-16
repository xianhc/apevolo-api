using System;
using System.IO;
using System.Threading.Tasks;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Model;
using Microsoft.AspNetCore.Http;

namespace ApeVolo.Api.Middleware;

public class NotFoundMiddleware
{
    private readonly RequestDelegate _next;

    public NotFoundMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        await _next(context);

        if (context.Response.StatusCode == StatusCodes.Status404NotFound && !context.Response.HasStarted)
        {
            // 设置ContentType
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(new ActionResultVm
            {
                Status = StatusCodes.Status404NotFound,
                ActionError = new ActionError(),
                Message = "请求失败，访问接口不存在",
                Path = context.Request.Path.Value?.ToLower()
            }.ToJson());

            return;
        }

        // // 继续处理请求
    }
}
