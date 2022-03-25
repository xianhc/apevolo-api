using System;
using System.Threading.Tasks;
using ApeVolo.Common.DI;
using ApeVolo.Common.WebApp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ApeVolo.Api.ActionExtension.Jwt;

/// <summary>
/// JWT校验
/// </summary>
public class TokenFilterAttribute : BaseActionFilter
{
    private readonly IHttpContextAccessor _accessor = AutofacHelper.GetService<IHttpContextAccessor>();
    private readonly ICurrentUser _currentUser = AutofacHelper.GetService<ICurrentUser>();

    /// <summary>
    /// Action执行之前执行
    /// </summary>
    /// <param name="context">过滤器上下文</param>
    public override async Task OnActionExecuting(ActionExecutingContext context)
    {
        try
        {
            //如果项目使用自定义jwt模式 需要在这里验证token是否有效或过期
            //还要增加白名单处理或无权限Attribute
            //if (context.ContainsFilter<无权限Attribute>()) return;

            //允许token为空(无权限接口) 权限接口已使用.net core内置鉴权
            //这里验证用户是在线 如果不需要用户在线功能 可把当前过滤器 登录接口保存在线用户方法注释
            //string token = _currentUser.GetToken();

            //if (!token.IsNullOrEmpty())
            //{
            //    var jwtUser = AsyncHelper.RunSync(() => RedisHelper.GetCacheAsync(RedisGlobalData.OnlineKey + token));
            //    if (jwtUser == null)
            //    {
            //        context.Result = Error("很抱歉，您已离线，请重新登录!", StatusCodes.Status401Unauthorized);
            //        return;
            //    }
            //}
        }
        catch (Exception ex)
        {
            context.Result = Error(ex.Message, StatusCodes.Status500InternalServerError);
        }

        await Task.CompletedTask;
    }
}