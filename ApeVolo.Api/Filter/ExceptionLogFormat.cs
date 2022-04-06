using System;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Helper;
using ApeVolo.Common.WebApp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace ApeVolo.Api.Filter;

/// <summary>
/// 统一输出日志格式
/// </summary>
public static class ExceptionLogFormat
{
    /// <summary>
    /// 自定义返回格式
    /// </summary>
    /// <param name="httpContext"></param>
    /// <param name="exception"></param>
    /// <param name="currentUserName"></param>
    /// <returns></returns>
    public static string WriteLog(HttpContext httpContext, Exception exception, string currentUserName)
    {
        var remoteIp = httpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";
        return string.Format("\r\n【异常信息】 : {0} \r\n" +
                             "【异常类型】 : {1} \r\n" +
                             "【请求类型】 : {2} \r\n" +
                             "【请求路径】 : {3} \r\n" +
                             "【请求内容】 : {4} \r\n" +
                             "【当前用户】 : {5} \r\n" +
                             "【当前IP】  : {6} \r\n" +
                             "【IP地址】  : {7} \r\n" +
                             "【浏览器】  : {8} \r\n" +
                             "【堆栈调用】 ：{9} \r\n" +
                             "【完整异常】 ：{10}", exception.Message, exception.GetType().Name,
            httpContext.Request.GetDisplayUrl(), httpContext.Request.Method,
            httpContext.Request.Body.ReadToString() ?? "", currentUserName,
            remoteIp, IpHelper.GetIpAddress(remoteIp),
            IpHelper.GetBrowserName(), exception.StackTrace,
            ExceptionHelper.GetExceptionAllMsg(exception));
    }
}