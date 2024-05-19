using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Ape.Volo.Common.ConfigOptions;
using Ape.Volo.Common.Exception;
using Ape.Volo.Common.Extention;
using Ape.Volo.Common.Helper;
using Ape.Volo.Common.Model;
using Ape.Volo.Common.SnowflakeIdHelper;
using Ape.Volo.Common.WebApp;
using Ape.Volo.Entity.Monitor;
using Ape.Volo.IBusiness.Interface.Monitor;
using Ape.Volo.IBusiness.Interface.System;
using IP2Region.Net.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shyjus.BrowserDetection;
using StackExchange.Profiling;
using static Ape.Volo.Api.Filter.ExceptionLogFormat;
using LogLevel = Ape.Volo.Common.Global.LogLevel;
using MiniProfiler = StackExchange.Profiling.MiniProfiler;

namespace Ape.Volo.Api.Filter;

public class GlobalExceptionFilter : IAsyncExceptionFilter
{
    private readonly IExceptionLogService _exceptionLogService;
    private readonly ISettingService _settingService;
    private readonly IBrowserDetector _browserDetector;
    private readonly ILogger<GlobalExceptionFilter> _logger;
    private readonly IHttpUser _httpUser;
    private readonly ISearcher _ipSearcher;
    private readonly bool _isMiniProfiler;

    public GlobalExceptionFilter(IExceptionLogService exceptionLogService, ISearcher searcher,
        IOptionsMonitor<Configs> configs,
        ISettingService settingService, IHttpUser httpUser, IBrowserDetector browserDetector,
        ILogger<GlobalExceptionFilter> logger)
    {
        _exceptionLogService = exceptionLogService;
        _settingService = settingService;
        _browserDetector = browserDetector;
        _logger = logger;
        _ipSearcher = searcher;
        _httpUser = httpUser;
        _isMiniProfiler = (configs?.CurrentValue ?? new Configs()).Middleware.MiniProfiler.Enabled;
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

        var remoteIp = context.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";
        var ipAddress = _ipSearcher.Search(remoteIp);
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
        if (_isMiniProfiler)
        {
            MiniProfiler.Current.CustomTiming("Errors：", throwMsg);
        }

        try
        {
            //记录日志
            _logger.LogError(WriteLog(context.HttpContext, remoteIp, ipAddress, context.Exception,
                _httpUser.Account,
                _browserDetector.Browser?.OS, _browserDetector.Browser?.DeviceType, _browserDetector.Browser?.Name,
                _browserDetector.Browser?.Version));
        }
        catch (Exception e)
        {
            //_logger.LogCritical("LogError出错:" + e.ToString());
            FileHelper.WriteLog(WriteLog(context.HttpContext, remoteIp, ipAddress, e,
                _httpUser.Account,
                _browserDetector.Browser?.OS, _browserDetector.Browser?.DeviceType, _browserDetector.Browser?.Name,
                _browserDetector.Browser?.Version));
        }

        var settingDto = await _settingService.FindSettingByName("IsExceptionLogSaveDB");
        if (settingDto != null && settingDto.Value.ToBool() && exceptionType != typeof(DemoRequestException))
        {
            //记录日志到数据库
            try
            {
                var log = CreateLog(context);
                if (log.IsNotNull())
                {
                    await _exceptionLogService.CreateAsync(log);
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(WriteLog(context.HttpContext, remoteIp, ipAddress, ex, _httpUser.Account,
                    _browserDetector.Browser?.OS, _browserDetector.Browser?.DeviceType, _browserDetector.Browser?.Name,
                    _browserDetector.Browser?.Version));
            }
        }
    }

    /// <summary>
    /// 创建异常对象
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    private ExceptionLog CreateLog(ExceptionContext context)
    {
        ExceptionLog log = null;
        try
        {
            var routeValues = context.ActionDescriptor.RouteValues;
            var httpContext = context.HttpContext;
            var remoteIp = httpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";
            var arguments = HttpHelper.GetAllRequestParams(httpContext);
            var descriptionAttribute = ((ControllerActionDescriptor)context.ActionDescriptor).MethodInfo
                .GetCustomAttributes(typeof(DescriptionAttribute), true)
                .OfType<DescriptionAttribute>()
                .FirstOrDefault();
            log = new ExceptionLog
            {
                Id = IdHelper.GetLongId(),
                CreateBy = _httpUser.Account,
                CreateTime = DateTime.Now,
                Area = routeValues["area"],
                Controller = routeValues["controller"],
                Action = routeValues["action"],
                Method = httpContext.Request.Method,
                Description = descriptionAttribute?.Description,
                RequestUrl = httpContext.Request.Path,
                RequestParameters = arguments.ToJson(),
                ExceptionMessage = context.Exception.Message,
                ExceptionMessageFull = ExceptionHelper.GetExceptionAllMsg(context.Exception),
                ExceptionStack = context.Exception.StackTrace,
                RequestIp = remoteIp,
                IpAddress = _ipSearcher.Search(remoteIp),
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
}
