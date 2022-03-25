using ApeVolo.Common.DI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace ApeVolo.Common.WebApp;

/// <summary>
/// Http请求上下文
/// </summary>
public static class HttpContextCore
{
    /// <summary>
    /// 当前上下文
    /// </summary>
    public static HttpContext CurrentHttpContext
    {
        get => AutofacHelper.GetService<IHttpContextAccessor>().HttpContext;
    }

    /// <summary>
    /// 网站文件根路径
    /// </summary>
    public static readonly string WebRootPath = AutofacHelper.GetService<IWebHostEnvironment>().WebRootPath;
}