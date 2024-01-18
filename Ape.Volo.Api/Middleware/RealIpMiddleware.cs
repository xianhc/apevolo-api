using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Ape.Volo.Api.Middleware;

/// <summary>
/// 远程IP中间件，nginx代理服务的时候需要使用才能通过RemoteIpAddress获取客户端真实IP
/// </summary>
public class RealIpMiddleware
{
    private readonly RequestDelegate _next;

    public RealIpMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public Task Invoke(HttpContext context)
    {
        var headers = context.Request.Headers;
        if (headers.TryGetValue("X-Forwarded-For", out var header))
        {
            context.Connection.RemoteIpAddress = IPAddress.Parse(header.ToString()
                .Split(',', StringSplitOptions.RemoveEmptyEntries)[0]);
        }

        return _next(context);
    }
}
