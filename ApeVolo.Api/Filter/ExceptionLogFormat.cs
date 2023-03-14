using System;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Helper;
using ApeVolo.Common.Resources;
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
    /// <param name="httpContext">http上下文</param>
    /// <param name="exception">异常类</param>
    /// <param name="currentUserName">当前用户名</param>
    /// <param name="operatingSystem">操作系统</param>
    /// <param name="deviceType">设备类型</param>
    /// <param name="browserName">浏览器名称</param>
    /// <param name="version">版本号</param>
    /// <returns></returns>
    public static string WriteLog(HttpContext httpContext, Exception exception, string currentUserName,
        string operatingSystem, string deviceType, string browserName, string version)
    {
        var remoteIp = httpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";
        return string.Format("【异常信息】 : {0} \r\n" +
                             "【异常类型】 : {1} \r\n" +
                             "【请求类型】 : {2} \r\n" +
                             "【请求路径】 : {3} \r\n" +
                             "【请求内容】 : {4} \r\n" +
                             "【当前用户】 : {5} \r\n" +
                             "【当前IP】  : {6} \r\n" +
                             "【IP地址】  : {7} \r\n" +
                             "【操作系统】  : {8} \r\n" +
                             "【设备类型】  : {9} \r\n" +
                             "【浏览器】  : {10} \r\n" +
                             "【版本号】  : {11} \r\n" +
                             "【堆栈调用】 ：{12} \r\n" +
                             "【完整异常】 ：{13}", exception.Message, exception.GetType().Name,
            httpContext.Request.GetDisplayUrl(), httpContext.Request.Method,
            httpContext.Request.Body.ReadToString() ?? "", currentUserName,
            remoteIp, IpHelper.GetIpAddress(remoteIp),
            operatingSystem, deviceType, browserName, version, exception.StackTrace,
            ExceptionHelper.GetExceptionAllMsg(exception));
    }

    /// <summary>
    /// 获取描述本地化语言
    /// </summary>
    /// <param name="description"></param>
    /// <param name="area"></param>
    /// <returns></returns>
    public static string GetResourcesDescription(string description, string area)
    {
        if (description.IsNullOrEmpty())
        {
            return "";
        }

        return description.Contains("{0}")
            ? Localized.Get(description, Localized.Get(area))
            : Localized.Get(description);
    }
}