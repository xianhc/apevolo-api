using ApeVolo.Common.DI;
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
    public static HttpContext CurrentHttpContext => AutofacHelper.GetService<IHttpContextAccessor>().HttpContext;
}