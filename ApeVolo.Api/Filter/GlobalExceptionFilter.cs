using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;
using ApeVolo.Common.Exception;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Global;
using ApeVolo.Common.Helper;
using ApeVolo.Common.Model;
using ApeVolo.Common.SnowflakeIdHelper;
using ApeVolo.Common.WebApp;
using ApeVolo.Entity.Monitor;
using ApeVolo.IBusiness.Interface.Monitor;
using ApeVolo.IBusiness.Interface.System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Shyjus.BrowserDetection;
using StackExchange.Profiling;
using static ApeVolo.Api.Filter.ExceptionLogFormat;
using LogLevel = ApeVolo.Common.Global.LogLevel;

namespace ApeVolo.Api.Filter;

public class GlobalExceptionFilter : IAsyncExceptionFilter
{
    private readonly IExceptionLogService _exceptionLogService;
    private readonly ISettingService _settingService;
    private readonly IBrowserDetector _browserDetector;
    private readonly ILogger<GlobalExceptionFilter> _logger;
    private readonly ApeContext _apeContext;

    public GlobalExceptionFilter(ApeContext apeContext, IExceptionLogService exceptionLogService,
        ISettingService settingService, IBrowserDetector browserDetector, ILogger<GlobalExceptionFilter> logger)
    {
        _apeContext = apeContext;
        _exceptionLogService = exceptionLogService;
        _settingService = settingService;
        _browserDetector = browserDetector;
        _logger = logger;
    }

    public async Task OnExceptionAsync(ExceptionContext context)
    {
        var exceptionType = context.Exception.GetType();
        var statusCode = StatusCodes.Status500InternalServerError;
        //自定义全局异常
        //if (exceptionType == typeof(ApevovoException))
        //{
        //  var ex = (ApevovoException)context.Exception;
        // statusCode = ex.StatusCode;
        // }
        if (exceptionType == typeof(BadRequestException)) //错误请求 无法处理
        {
            statusCode = StatusCodes.Status400BadRequest;
        }

        string throwMsg = context.Exception.Message; //错误信息
        var actionError = new ActionError() { Errors = new Dictionary<string, string>() };
        context.Result = new ContentResult
        {
            Content = new ActionResultVm
            {
                Status = statusCode,
                ActionError = actionError,
                Message = throwMsg,
                Timestamp = DateTime.Now.ToUnixTimeStampMillisecond().ToString(),
                Path = context.HttpContext.Request.Path.Value?.ToLower()
            }.ToJson(),
            ContentType = "application/json; charset=utf-8",
            StatusCode = statusCode
        };
        if (_apeContext.Configs.IsMiniProfiler)
        {
            MiniProfiler.Current.CustomTiming("Errors：", throwMsg);
        }

        //记录日志
        _logger.LogError(WriteLog(context.HttpContext, context.Exception, _apeContext.HttpUser.Account,
            _browserDetector.Browser?.OS, _browserDetector.Browser?.DeviceType, _browserDetector.Browser?.Name,
            _browserDetector.Browser?.Version), context.Exception);
        var settingDto = await _settingService.FindSettingByName("IsExceptionLogSaveDB");
        if (settingDto != null && settingDto.Value.ToBool() && exceptionType != typeof(DemoRequestException))
        {
            //记录日志到数据库
            try
            {
                var log = CreateLog(context);
                if (log.IsNotNull())
                {
                    await _exceptionLogService.AddEntityAsync(log);
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(WriteLog(context.HttpContext, ex, _apeContext.HttpUser.Account,
                    _browserDetector.Browser?.OS, _browserDetector.Browser?.DeviceType, _browserDetector.Browser?.Name,
                    _browserDetector.Browser?.Version));
                ConsoleHelper.WriteLine(ex.Message, ConsoleColor.Red);
            }
        }
    }

    /// <summary>
    /// 创建审计对象
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    private ExceptionLog CreateLog(ExceptionContext context)
    {
        var routeValues = context.ActionDescriptor.RouteValues;
        var desc =
            ((ControllerActionDescriptor)context.ActionDescriptor).MethodInfo.GetCustomAttribute(
                typeof(DescriptionAttribute), true);

        ExceptionLog log = null;
        try
        {
            var httpContext = context.HttpContext;
            var remoteIp = httpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";
            var arguments = HttpHelper.GetAllRequestParams(httpContext);
            var description = desc == null ? "" : ((DescriptionAttribute)desc).Description;
            log = new ExceptionLog
            {
                Id = IdHelper.GetLongId(),
                CreateBy = _apeContext.HttpUser.Account,
                CreateTime = DateTime.Now,
                Area = routeValues["area"],
                Controller = routeValues["controller"],
                Action = routeValues["action"],
                Method = httpContext.Request.Method,
                Description = GetResourcesDescription(description, routeValues["area"]),
                RequestUrl = httpContext.Request.GetDisplayUrl(),
                RequestParameters = arguments.ToJson(),
                ExceptionMessage = context.Exception.Message,
                ExceptionMessageFull = ExceptionHelper.GetExceptionAllMsg(context.Exception),
                ExceptionStack = context.Exception.StackTrace,
                RequestIp = remoteIp,
                IpAddress = IpHelper.GetIpAddress(remoteIp),
                LogLevel = (int)LogLevel.Debug,
                OperatingSystem = _browserDetector.Browser?.OS,
                DeviceType = _browserDetector.Browser?.DeviceType,
                BrowserName = _browserDetector.Browser?.Name,
                Version = _browserDetector.Browser?.Version
            };
        }
        catch
        {
            // ignored
        }

        return log;
    }

    /// <summary>
    /// 根据状态码获取错误名称
    /// </summary>
    /// <param name="statusCode"></param>
    /// <returns></returns>
    private static string GetExceptionError(int statusCode)
    {
        var errorText = statusCode switch
        {
            StatusCodes.Status500InternalServerError => "Status500InternalServerError",
            StatusCodes.Status400BadRequest => "Status400BadRequest",
            _ => "Status500InternalServerError"
        };

        return errorText;
    }
}
