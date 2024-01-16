using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ApeVolo.Common.AttributeExt;
using ApeVolo.Common.ConfigOptions;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Global;
using ApeVolo.Common.WebApp;
using ApeVolo.IBusiness.Interface.Permission;
using ApeVolo.IBusiness.Interface.System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;

namespace ApeVolo.Api.Authentication.Jwt;

/// <summary>
/// 权限授权处理器
/// </summary>
public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    /// <summary>
    /// 验证方案提供对象
    /// </summary>
    public IAuthenticationSchemeProvider Schemes { get; set; }

    private readonly IPermissionService _permissionService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ISettingService _settingService;
    private readonly JwtAuthOption _jwtOptions;
    private readonly ApeContext _apeContext;

    /// <summary>
    /// 构造函数注入
    /// </summary>
    /// <param name="schemes"></param>
    /// <param name="httpContextAccessor"></param>
    /// <param name="permissionService"></param>
    /// <param name="apeContext"></param>
    /// <param name="settingService"></param>
    public PermissionHandler(IAuthenticationSchemeProvider schemes, IHttpContextAccessor httpContextAccessor,
        IPermissionService permissionService, ISettingService settingService, ApeContext apeContext)
    {
        _httpContextAccessor = httpContextAccessor;
        Schemes = schemes;
        _permissionService = permissionService;
        _settingService = settingService;
        _apeContext = apeContext;
        _jwtOptions = apeContext.Configs.JwtAuthOptions;
    }

    // 重写异步处理程序
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        var isMatchRole = false;
        LoginUserInfo loginUserInfo = null;
        var httpContext = _httpContextAccessor?.HttpContext;
        //请求Url
        if (httpContext != null)
        {
            var questUrl = httpContext.Request.Path.Value?.ToLower();
            //判断请求是否停止
            var handlers = httpContext.RequestServices.GetRequiredService<IAuthenticationHandlerProvider>();
            foreach (var scheme in await Schemes.GetRequestHandlerSchemesAsync())
            {
                if (await handlers.GetHandlerAsync(httpContext, scheme.Name) is IAuthenticationRequestHandler
                        handler && await handler.HandleRequestAsync())
                {
                    context.Fail();
                    return;
                }
            }

            //判断请求是否拥有凭据，即有没有登录
            var defaultAuthenticate = await Schemes.GetDefaultAuthenticateSchemeAsync();
            if (defaultAuthenticate != null)
            {
                var result = await httpContext.AuthenticateAsync(defaultAuthenticate.Name);
                //result.Principal不为空代表http状态存在
                if (result is { Principal: not null })
                {
                    httpContext.User = result.Principal;

                    #region 判断jwt令牌是否已过期

                    //判断jwt令牌是否已过期
                    var expirationClaim =
                        httpContext.User.Claims.FirstOrDefault(s => s.Type == AuthConstants.JwtClaimTypes.Exp);
                    if (expirationClaim != null)
                    {
                        var expTime = Convert.ToInt64(expirationClaim.Value).TicksToDateTime();
                        var nowTime = DateTime.Now.ToLocalTime();
                        if (expTime < nowTime)
                        {
                            context.Fail();
                            return;
                        }
                    }

                    #endregion

                    #region 用户缓存信息是否已过期

                    loginUserInfo = await _apeContext.Cache.GetAsync<LoginUserInfo>(
                        GlobalConstants.CacheKey.OnlineKey +
                        _apeContext.HttpUser.JwtToken.ToMd5String16());
                    if (loginUserInfo == null)
                    {
                        context.Fail();
                        return;
                    }

                    #region 判断IP是否变化

                    var ipClaim =
                        httpContext.User.Claims.FirstOrDefault(s => s.Type == AuthConstants.JwtClaimTypes.Ip);
                    if (ipClaim != null)
                    {
                        var remoteIp = httpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";
                        if (!remoteIp.Equals(ipClaim.Value))
                        {
                            //IP已发生变化，执行系统处理逻辑
                        }
                    }

                    #endregion

                    #endregion

                    #region ApeVoloOnlineAttribute属性直接放行

                    try
                    {
                        //在线特性，接口拥有ApeVoloOnlineAttribute 直接放行
                        if (context.Resource.IsNotNull())
                        {
                            var endpointFeature = (IEndpointFeature)((DefaultHttpContext)context.Resource)
                                ?.Features.FirstOrDefault(x =>
                                    x.Key.FullName == typeof(IEndpointFeature).FullName).Value;
                            if (endpointFeature != null)
                            {
                                var apeVoloOnline =
                                    endpointFeature.Endpoint?.Metadata.FirstOrDefault(x =>
                                            x.GetType() == typeof(ApeVoloOnlineAttribute)) as
                                        ApeVoloOnlineAttribute;
                                if (apeVoloOnline.IsNotNull())
                                {
                                    context.Succeed(requirement);
                                    return;
                                }
                            }
                        }
                    }
                    catch
                    {
                        // ignored
                    }

                    #endregion

                    #region 验证用户权限

                    //验证权限 需要保持用户最新权限信息可以刷新在线用户OnlineUser.CurrentPermission
                    if ((await _settingService.FindSettingByName("IsRealTimeAuthentication")).Value.ToBool())
                    {
                        //重新查询用户权限 自行修改判断
                        var permissionList =
                            await _permissionService.QueryUserPermissionAsync(_apeContext.HttpUser.Id);
                    }

                    //验证url
                    foreach (var url in loginUserInfo.CurrentPermission.Urls)
                    {
                        try
                        {
                            if (Regex.Match(questUrl ?? string.Empty, url.ToLower()).Value == questUrl)
                            {
                                isMatchRole = true;
                                break;
                            }
                        }
                        catch
                        {
                            // ignored
                        }
                    }

                    //权限url验证失败，再验证权接口是否有权限标识符
                    if (!isMatchRole)
                    {
                        try
                        {
                            if (context.Resource.IsNotNull())
                            {
                                var endpointFeature = (IEndpointFeature)((DefaultHttpContext)context.Resource)
                                    .Features.FirstOrDefault(x =>
                                        x.Key.FullName == typeof(IEndpointFeature).FullName).Value;
                                var apeVoloAuthorize =
                                    endpointFeature.Endpoint?.Metadata.FirstOrDefault(x =>
                                            x.GetType() == typeof(ApeVoloAuthorizeAttribute)) as
                                        ApeVoloAuthorizeAttribute;
                                // .net core 3.1获取的方式 
                                //ApeVoloAuthorizeAttribute apeVoloAuthorize = ((Endpoint)context.Resource).Metadata.FirstOrDefault(x => x.GetType() == typeof(ApeVoloAuthorizeAttribute)) as ApeVoloAuthorizeAttribute;
                                if (apeVoloAuthorize.IsNotNull())
                                {
                                    if (apeVoloAuthorize != null && apeVoloAuthorize.Roles.Any(role =>
                                            loginUserInfo.CurrentPermission.Roles.Contains(role)))
                                    {
                                        isMatchRole = true;
                                    }
                                }
                            }
                        }
                        catch
                        {
                            // ignored
                        }
                    }

                    if (!isMatchRole)
                    {
                        context.Fail();
                        return;
                    }

                    #endregion

                    context.Succeed(requirement);
                    return;
                }
            }

            //判断没有登录时，是否访问登录的url,并且是Post请求，并且是form表单提交类型，否则为失败
            if (questUrl != null &&
                !questUrl.Equals(_jwtOptions.LoginPath.ToLower(), StringComparison.Ordinal) &&
                (!httpContext.Request.Method.Equals("POST") || !httpContext.Request.HasFormContentType))
            {
                context.Fail();
                return;
            }
        }

        context.Fail();
    }
}
