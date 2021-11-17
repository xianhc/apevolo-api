using ApeVolo.Common.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using ApeVolo.Common.Extention;
using Microsoft.AspNetCore.Http;
using ApeVolo.Common.WebApp;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ApeVolo.Api.ActionExtension
{
    public class BaseActionFilter : Attribute, IAsyncActionFilter
    {
        public async virtual Task OnActionExecuting(ActionExecutingContext context)
        {
            await Task.CompletedTask;
        }

        public async virtual Task OnActionExecuted(ActionExecutedContext context)
        {
            await Task.CompletedTask;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            await OnActionExecuting(context);
            if (context.Result == null)
            {
                var nextContext = await next();
                await OnActionExecuted(nextContext);
            }
        }

        /// <summary>
        /// 返回JSON
        /// </summary>
        /// <param name="json">json字符串</param>
        /// <returns></returns>
        public ContentResult JsonContent(string json)
        {
            return new ContentResult
                {Content = json, StatusCode = StatusCodes.Status200OK, ContentType = "application/json; charset=utf-8"};
        }

        /// <summary>
        /// 返回成功
        /// </summary>
        /// <returns></returns>
        public ContentResult Success()
        {
            ActionResultVm res = new ActionResultVm
            {
                Message = "请求成功！",
                Path = HttpContextCore.CurrentHttpContext.Request.Path.Value?.ToLower()
            };

            return JsonContent(res.ToJson());
        }

        /// <summary>
        /// 返回成功
        /// </summary>
        /// <param name="msg">消息</param>
        /// <returns></returns>
        public ContentResult Success(string msg)
        {
            ActionResultVm res = new ActionResultVm
            {
                Message = msg,
                Path = HttpContextCore.CurrentHttpContext.Request.Path.Value?.ToLower()
            };

            return JsonContent(res.ToJson());
        }

        /// <summary>
        /// 返回成功
        /// </summary>
        /// <param name="data">返回的数据</param>
        /// <returns></returns>
        public ContentResult Success<T>(List<T> data)
        {
            ActionResultVm<T> res = new ActionResultVm<T>
            {
                Content = data,
                TotalElements = 0
            };

            return JsonContent(res.ToJson());
        }

        /// <summary>
        /// 返回错误
        /// </summary>
        /// <returns></returns>
        public ContentResult Error()
        {
            ActionResultVm res = new ActionResultVm
            {
                Status = StatusCodes.Status400BadRequest,
                Error = "BadRequest",
                Message = "请求失败！",
                Path = HttpContextCore.CurrentHttpContext.Request.Path.Value?.ToLower()
            };

            return JsonContent(res.ToJson());
        }

        /// <summary>
        /// 返回错误
        /// </summary>
        /// <param name="msg">错误提示</param>
        /// <returns></returns>
        public ContentResult Error(string msg)
        {
            ActionResultVm res = new ActionResultVm
            {
                Status = StatusCodes.Status400BadRequest,
                Error = "BadRequest",
                Message = msg,
                Path = HttpContextCore.CurrentHttpContext.Request.Path.Value?.ToLower()
            };

            return JsonContent(res.ToJson());
        }

        /// <summary>
        /// 返回错误
        /// </summary>
        /// <param name="msg">错误提示</param>
        /// <param name="errorCode">错误代码</param>
        /// <returns></returns>
        public ContentResult Error(string msg, int errorCode)
        {
            ActionResultVm res = new ActionResultVm
            {
                Status = errorCode,
                Error = "BadRequest",
                Message = msg,
                Path = HttpContextCore.CurrentHttpContext.Request.Path.Value?.ToLower()
            };

            return JsonContent(res.ToJson());
        }
    }
}