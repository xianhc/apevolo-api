using System;
using Ape.Volo.Common.Extention;
using Ape.Volo.Common.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace Ape.Volo.Api.Filter;

/// <summary>
/// 统一输出日志格式
/// </summary>
public static class ExceptionLogFormat
{
    /// <summary>
    /// 自定义返回格式
    /// </summary>
    /// <param name="httpContext">http上下文</param>
    ///  <param name="remoteIp">http上下文</param>
    ///  <param name="ipAddress">http上下文</param>
    /// <param name="exception">异常类</param>
    /// <param name="currentUserName">当前用户名</param>
    /// <param name="operatingSystem">操作系统</param>
    /// <param name="deviceType">设备类型</param>
    /// <param name="browserName">浏览器名称</param>
    /// <param name="version">版本号</param>
    /// <returns></returns>
    public static string WriteLog(HttpContext httpContext, string remoteIp, string ipAddress, Exception exception,
        string currentUserName,
        string operatingSystem, string deviceType, string browserName, string version)
    {
        return string.Format("\r\n【异常信息】 : {0} \r\n" +
                             "【异常类型】 : {1} \r\n" +
                             "【请求路径】 : {2} \r\n" +
                             "【请求类型】 : {3} \r\n" +
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
            remoteIp, ipAddress,
            operatingSystem, deviceType, browserName, version, exception.StackTrace,
            ExceptionHelper.GetExceptionAllMsg(exception));
    }
}
