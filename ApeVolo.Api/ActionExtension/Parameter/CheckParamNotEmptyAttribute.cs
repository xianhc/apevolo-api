using System;
using System.Collections.Generic;
using System.Linq;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Helper;
using ApeVolo.Common.Model;
using ApeVolo.Common.WebApp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ApeVolo.Api.ActionExtension.Parameter
{
    /// <summary>
    /// 参数非空校验
    /// </summary>
    public class CheckParamNotEmptyAttribute : Attribute, IActionFilter
    {
        private readonly List<string> _paramters;

        public CheckParamNotEmptyAttribute(params string[] paramters)
        {
            _paramters = paramters.ToList();
        }

        /// <summary>
        /// Action执行之前执行
        /// </summary>
        /// <param name="filterContext">过滤器上下文</param>
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var allParamters = HttpHelper.GetAllRequestParams(filterContext.HttpContext);
            var needParamters = _paramters.Where(x =>
            {
                if (!allParamters.ContainsKey(x))
                    return true;
                else
                    return allParamters[x].IsNullOrEmpty();
            }).ToList();
            if (needParamters.Count != 0)
            {
                ActionResultVm res = new ActionResultVm
                {
                    Status = StatusCodes.Status400BadRequest,
                    Error = "BadRequest",
                    Message = $"参数:{string.Join(",", needParamters)}不能为空！",
                    Path = HttpContextCore.CurrentHttpContext.Request.Path.Value?.ToLower()
                };
                filterContext.Result = new ContentResult
                {
                    StatusCode = StatusCodes.Status400BadRequest, Content = res.ToJson(),
                    ContentType = "application/json;charset=utf-8"
                };
            }
        }

        /// <summary>
        /// Action执行完毕之后执行
        /// </summary>
        /// <param name="filterContext"></param>
        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }
    }
}