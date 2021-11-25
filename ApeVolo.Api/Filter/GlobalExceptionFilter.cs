using ApeVolo.Common.Exception;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Global;
using ApeVolo.Common.Helper;
using ApeVolo.Common.Model;
using ApeVolo.Common.SnowflakeIdHelper;
using ApeVolo.Common.WebApp;
using ApeVolo.IBusiness.Interface.Logs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using StackExchange.Profiling;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;
using ApeVolo.Entity.Do.Logs;
using ApeVolo.IBusiness.Interface.Core;
using Microsoft.AspNetCore.Http.Extensions;

namespace ApeVolo.Api.Filter
{
    public class GlobalExceptionFilter : IAsyncExceptionFilter
    {
        private readonly ILogService _logService;
        private readonly ILogger<GlobalExceptionFilter> _loggerHelper;
        private readonly ICurrentUser _currentUser;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISettingService _settingService;

        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(typeof(GlobalExceptionFilter));


        public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> loggerHelper, ICurrentUser currentUser,
            ILogService logService, IHttpContextAccessor httpContextAccessor, ISettingService settingService)
        {
            _logService = logService;
            _loggerHelper = loggerHelper;
            _currentUser = currentUser;
            _httpContextAccessor = httpContextAccessor;
            _settingService = settingService;
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
            context.Result = new ContentResult
            {
                Content = new ActionResultVm
                {
                    Status = statusCode,
                    Error = GetExceptionError(statusCode),
                    Message = throwMsg,
                    Path = HttpContextCore.CurrentHttpContext.Request.Path.Value?.ToLower()
                }.ToJson(),
                ContentType = "application/json; charset=utf-8",
                StatusCode = statusCode
            };
            if (AppSettings.GetValue("IsMiniProfiler").ToBool())
            {
                MiniProfiler.Current.CustomTiming("Errors：", throwMsg);
            }

            //记录日志
            Log.Error(WriteLog(context));
            if ((await _settingService.FindSettingByName("IsExceptionLogSaveDB")).Value.ToBool() &&
                exceptionType != typeof(DemoRequestException))
            {
                //记录日志到数据库
                try
                {
                    Log log = CreateLog(context);
                    if (log.IsNotNull())
                    {
                        await _logService.CreateAsync(log);
                    }
                }
                catch
                {
                    // ignored
                }
            }

            //await Task.CompletedTask;
        }

        /// <summary>
        /// 自定义返回格式
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private string WriteLog(ExceptionContext context)
        {
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
                                 "【完整异常】 ：{10}", context.Exception.Message, context.Exception.GetType().Name,
                HttpContextCore.CurrentHttpContext.Request.GetDisplayUrl() ?? "", context.HttpContext.Request.Method,
                HttpContextCore.CurrentHttpContext.Request.Body.ReadToString() ?? "", _currentUser.Name ?? "",
                IpHelper.GetIp(), IpHelper.GetIpAddress(),
                IpHelper.GetBrowserName(), context.Exception.StackTrace,
                ExceptionHelper.GetExceptionAllMsg(context.Exception));
        }

        /// <summary>
        /// 创建审计对象
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private Log CreateLog(ExceptionContext context)
        {
            var routeValues = context.ActionDescriptor.RouteValues;
            Attribute desc =
                ((ControllerActionDescriptor)context.ActionDescriptor).MethodInfo.GetCustomAttribute(
                    typeof(DescriptionAttribute), true);

            //var dics = context.ActionArguments;
            Log log = null;
            try
            {
                var discs = HttpHelper.GetAllRequestParams(context.HttpContext);
                log = new Log
                {
                    Id = IdHelper.GetId(),
                    CreateBy = _currentUser.Id,
                    CreateTime = DateTime.Now,
                    Area = routeValues["area"],
                    Controller = routeValues["controller"],
                    Action = routeValues["action"],
                    Method = context.HttpContext.Request.Method,
                    Description = desc == null ? "" : ((DescriptionAttribute)desc).Description,
                    RequestUrl = HttpContextCore.CurrentHttpContext.Request.GetDisplayUrl() ?? "",
                    RequestParameters = discs.ToJson(),
                    BrowserInfo = IpHelper.GetBrowserName(),
                    RequestIp = IpHelper.GetIp(),
                    IpAddress = IpHelper.GetIpAddress(),
                    LogLevel = (int)Common.Global.LogLevel.Debug,
                    ExceptionMessage = context.Exception.Message,
                    ExceptionMessageFull = ExceptionHelper.GetExceptionAllMsg(context.Exception),
                    ExceptionStack = context.Exception.StackTrace
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
}