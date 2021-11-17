using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Helper;
using ApeVolo.Common.SnowflakeIdHelper;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.Controllers;
using ApeVolo.Common.WebApp;
using ApeVolo.Entity.Do.Logs;
using ApeVolo.IBusiness.Interface.Core;
using ApeVolo.IBusiness.Interface.Logs;
using Microsoft.AspNetCore.Http.Extensions;

namespace ApeVolo.Api.Filter
{
    /// <summary>
    /// 审计过滤器
    /// </summary>
    public class AuditingFilter : IAsyncActionFilter
    {
        private readonly IAuditLogService _auditInfoService;
        private readonly ICurrentUser _currentUser;
        private readonly ISettingService _settingService;

        public AuditingFilter(IAuditLogService auditInfoService, ICurrentUser currentUser,
            ISettingService settingService)
        {
            _auditInfoService = auditInfoService;
            _currentUser = currentUser;
            _settingService = settingService;
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
                //执行结果

                if ((await _settingService.FindSettingByName("IsAuditLogSaveDB")).Value.ToBool())
                {
                    var result = resultContext.Result;
                    //var routeValues = context.ActionDescriptor.RouteValues;
                    //var action = routeValues["action"] ?? "";
                    //var reqUrl = HttpContextCore.RequestUrlPath;
                    if (HttpContextCore.CurrentHttpContext != null)
                    {
                        var reqUrlPath = HttpContextCore.CurrentHttpContext.Request.Path.Value?.ToLower();
                        var settingDto = await _settingService.FindSettingByName("LgnoreAuditLogUrlPath");
                        var lgnoreAuditLogUrlPathList = settingDto.Value.Split("|");
                        if (!lgnoreAuditLogUrlPathList.Contains(reqUrlPath))
                        {
                            AuditLog auditInfo = CreateAuditLog(context);
                            if (result.GetType().FullName == "Microsoft.AspNetCore.Mvc.ObjectResult")
                            {
                                auditInfo.ResponseData = ((ObjectResult) result).Value.ToString();
                            }
                            else if (result.GetType().FullName == "Microsoft.AspNetCore.Mvc.FileContentResult")
                            {
                                auditInfo.ResponseData = ((FileContentResult) result).FileDownloadName;
                            }
                            else
                            {
                                auditInfo.ResponseData = ((ContentResult) result).Content;
                            }

                            //用时
                            auditInfo.ExecutionDuration = Convert.ToInt32(sw.ElapsedMilliseconds);
                            await _auditInfoService.CreateAsync(auditInfo);
                            //Task addLgo = Task.Factory.StartNew(async () =>
                            //{
                            //    //auditInfo.ResponseData = "{}";
                            //    await _auditInfoService.CreateAsync(auditInfo);
                            //});
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex.Message,
                    new[] {"AuditingFilter", DateTime.Now.ToString("yyyy-MM-dd")});
                //ConsoleHelper.WriteLine(ex.Message, ConsoleColor.Red);
                // ignored
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
            Attribute desc =
                ((ControllerActionDescriptor) context.ActionDescriptor).MethodInfo.GetCustomAttribute(
                    typeof(DescriptionAttribute), true);

            var decs = HttpHelper.GetAllRequestParams(context.HttpContext); //context.ActionArguments;

            var auditLog = new AuditLog
            {
                Id = IdHelper.GetId(),
                CreateBy = _currentUser.Name ?? "",
                CreateTime = DateTime.Now,
                Area = routeValues["area"],
                Controller = routeValues["controller"],
                Action = routeValues["action"],
                Method = context.HttpContext.Request.Method,
                Description = desc == null ? "" : ((DescriptionAttribute) desc).Description,
                RequestUrl = HttpContextCore.CurrentHttpContext.Request.GetDisplayUrl() ?? "",
                RequestParameters = decs.ToJson(),
                BrowserInfo = IpHelper.GetBrowserName(),
                RequestIp = IpHelper.GetIp(),
                IpAddress = IpHelper.GetIpAddress()
            };


            var reqUrl = HttpContextCore.CurrentHttpContext.Request.Path.Value?.ToLower();
            if (reqUrl is "/auth/login")
            {
                var (_, value) = decs.SingleOrDefault(k => k.Key == "username");
                if (!value.IsNullOrEmpty())
                {
                    auditLog.CreateBy = value.ToString();
                }
            }

            return auditLog;
        }
    }
}