using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Ape.Volo.Common.AttributeExt;
using Ape.Volo.Common.Extention;
using Ape.Volo.Common.Helper;
using Ape.Volo.Common.SnowflakeIdHelper;
using Ape.Volo.Common.WebApp;
using Ape.Volo.Entity.Monitor;
using Ape.Volo.IBusiness.Interface.Monitor;
using Ape.Volo.IBusiness.Interface.System;
using IP2Region.Net.Abstractions;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Shyjus.BrowserDetection;

namespace Ape.Volo.Api.Filter;

/// <summary>
/// 审计过滤器
/// </summary>
public class AuditingFilter : IAsyncActionFilter
{
    private readonly IAuditLogService _auditInfoService;
    private readonly IHttpUser _httpUser;
    private readonly ISettingService _settingService;
    private readonly IBrowserDetector _browserDetector;
    private readonly ILogger<AuditingFilter> _logger;
    private readonly ISearcher _ipSearcher;

    public AuditingFilter(IAuditLogService auditInfoService, IHttpUser httpUser, ISearcher searcher,
        ISettingService settingService, IBrowserDetector browserDetector, ILogger<AuditingFilter> logger)
    {
        _auditInfoService = auditInfoService;
        _httpUser = httpUser;
        _settingService = settingService;
        _browserDetector = browserDetector;
        _logger = logger;
        _ipSearcher = searcher;
    }

    public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        return ExecuteAuditing(context, next);
    }

    /// <summary>
    /// 执行审计功能
    /// </summary>
    /// <param name="context"></param>
    /// <param name="next"></param>
    /// <returns></returns>
    private async Task ExecuteAuditing(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        try
        {
            var sw = new Stopwatch();
            sw.Start();
            var resultContext = await next();
            sw.Stop();

            var action = (ControllerActionDescriptor)context.ActionDescriptor;
            if (action.MethodInfo.IsDefined(typeof(NotAuditAttribute), false))
            {
                return;
            }

            //执行结果
            //var action = context.ActionDescriptor as ControllerActionDescriptor;
            //var isTrue = action.MethodInfo.IsDefined(typeof(DescriptionAttribute), false);
            if ((await _settingService.FindSettingByName("IsAuditLogSaveDB")).Value.ToBool())
            {
                var result = resultContext.Result;
                if (context.HttpContext.IsNotNull() && result.IsNotNull())
                {
                    var auditInfo = CreateAuditLog(context);
                    switch (result?.GetType().FullName)
                    {
                        case "Microsoft.AspNetCore.Mvc.ObjectResult":
                        {
                            var value = ((ObjectResult)result).Value;
                            if (value != null)
                                auditInfo.ResponseData = value.ToString();
                            break;
                        }
                        case "Microsoft.AspNetCore.Mvc.FileContentResult":
                            auditInfo.ResponseData = ((FileContentResult)result).FileDownloadName;
                            break;
                        default:
                            auditInfo.ResponseData = ((ContentResult)result)?.Content;
                            break;
                    }

                    //用时
                    auditInfo.ExecutionDuration = Convert.ToInt32(sw.ElapsedMilliseconds);
                    await _auditInfoService.CreateAsync(auditInfo);
                }
            }
        }
        catch (Exception ex)
        {
            var remoteIp = context.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";
            var ipAddress = _ipSearcher.Search(remoteIp);
            _logger.LogCritical(ExceptionLogFormat.WriteLog(context.HttpContext, remoteIp, ipAddress, ex,
                _httpUser?.Account,
                _browserDetector.Browser?.OS, _browserDetector.Browser?.DeviceType, _browserDetector.Browser?.Name,
                _browserDetector.Browser?.Version));
        }
    }

    /// <summary>
    /// 创建审计对象
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    private AuditLog CreateAuditLog(ActionExecutingContext context)
    {
        var routeValues = context.ActionDescriptor.RouteValues;

        var httpContext = context.HttpContext;
        var remoteIp = httpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";
        var arguments = HttpHelper.GetAllRequestParams(httpContext); //context.ActionArguments;
        var descriptionAttribute = ((ControllerActionDescriptor)context.ActionDescriptor).MethodInfo
            .GetCustomAttributes(typeof(DescriptionAttribute), true)
            .OfType<DescriptionAttribute>()
            .FirstOrDefault();
        var auditLog = new AuditLog
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
            RequestIp = remoteIp,
            IpAddress = _ipSearcher.Search(remoteIp),
            OperatingSystem = _browserDetector.Browser?.OS,
            DeviceType = _browserDetector.Browser?.DeviceType,
            BrowserName = _browserDetector.Browser?.Name,
            Version = _browserDetector.Browser?.Version
        };


        var reqUrl = httpContext.Request.Path.Value?.ToLower();
        if (reqUrl is "/auth/login")
        {
            var (_, value) = arguments.SingleOrDefault(k => k.Key == "username");
            if (!value.IsNullOrEmpty())
            {
                auditLog.CreateBy = value.ToString();
            }
        }

        return auditLog;
    }
}
