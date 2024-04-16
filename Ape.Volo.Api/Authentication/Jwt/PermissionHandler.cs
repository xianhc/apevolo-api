using System;
using System.Linq;
using System.Threading.Tasks;
using Ape.Volo.Common.ConfigOptions;
using Ape.Volo.Common.Extention;
using Ape.Volo.Common.Global;
using Ape.Volo.Common.WebApp;
using Ape.Volo.IBusiness.Interface.Permission;
using Ape.Volo.IBusiness.Interface.System;
using IP2Region.Net.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Shyjus.BrowserDetection;

namespace Ape.Volo.Api.Authentication.Jwt;

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
    private readonly IUserService _userService;
    private readonly ISettingService _settingService;
    private readonly ApeContext _apeContext;
    private readonly IBrowserDetector _browserDetector;
    private readonly ISearcher _ipSearcher;
    private readonly ITokenBlacklistService _tokenBlacklistService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="schemes"></param>
    /// <param name="httpContextAccessor"></param>
    /// <param name="permissionService"></param>
    /// <param name="userService"></param>
    /// <param name="settingService"></param>
    /// <param name="apeContext"></param>
    /// <param name="browserDetector"></param>
    /// <param name="searcher"></param>
    /// <param name="tokenBlacklistService"></param>
    public PermissionHandler(IAuthenticationSchemeProvider schemes, IHttpContextAccessor httpContextAccessor,
        IPermissionService permissionService, IUserService userService, ISettingService settingService,
        ApeContext apeContext, IBrowserDetector browserDetector, ISearcher searcher,
        ITokenBlacklistService tokenBlacklistService)
    {
        _httpContextAccessor = httpContextAccessor;
        Schemes = schemes;
        _permissionService = permissionService;
        _settingService = settingService;
        _apeContext = apeContext;
        _userService = userService;
        _browserDetector = browserDetector;
        _ipSearcher = searcher;
        _tokenBlacklistService = tokenBlacklistService;
    }

    // 重写异步处理程序
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        var isMatchRole = false;
        var httpContext = _httpContextAccessor?.HttpContext;
        //请求Url
        if (httpContext != null)
        {
            var requestPath = httpContext.Request.Path.Value?.ToLower();
            var requestMethod = httpContext.Request.Method.ToLower();

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
                            await _apeContext.Cache.RemoveAsync(GlobalConstants.CacheKey.OnlineKey +
                                                                _apeContext.HttpUser.JwtToken.ToMd5String16());
                            context.Fail();
                            return;
                        }
                    }

                    #endregion

                    #region 用户缓存信息是否已过期

                    var loginUserInfo = await _apeContext.Cache.GetAsync<LoginUserInfo>(
                        GlobalConstants.CacheKey.OnlineKey +
                        _apeContext.HttpUser.JwtToken.ToMd5String16());
                    if (loginUserInfo == null)
                    {
                        var tokenMd5 = _apeContext.HttpUser.JwtToken.ToMd5String16();
                        var tokenBlacklist = await _tokenBlacklistService.TableWhere(x => x.AccessToken == tokenMd5)
                            .FirstAsync();
                        if (tokenBlacklist.IsNotNull())
                        {
                            context.Fail();
                            return;
                        }

                        var netUser = await _userService.QueryByIdAsync(_apeContext.HttpUser.Id);
                        if (netUser.IsNull())
                        {
                            context.Fail();
                            return;
                        }

                        var remoteIp = httpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";
                        var onlineUser = new LoginUserInfo
                        {
                            UserId = netUser.Id,
                            Account = netUser.Username,
                            NickName = netUser.NickName,
                            DeptId = netUser.DeptId,
                            DeptName = netUser.Dept.Name,
                            Ip = remoteIp,
                            Address = _ipSearcher.Search(remoteIp),
                            OperatingSystem = _browserDetector.Browser?.OS,
                            DeviceType = _browserDetector.Browser?.DeviceType,
                            BrowserName = _browserDetector.Browser?.Name,
                            Version = _browserDetector.Browser?.Version,
                            LoginTime = DateTime.Now,
                            IsAdmin = netUser.IsAdmin,
                            AccessToken = _apeContext.HttpUser.JwtToken
                        };
                        var onlineKey = onlineUser.AccessToken.ToMd5String16();
                        var isTrue = await _apeContext.Cache.SetAsync(
                            GlobalConstants.CacheKey.OnlineKey + onlineKey, onlineUser, TimeSpan.FromHours(2),
                            null);
                        if (!isTrue)
                        {
                            context.Fail();
                            return;
                        }
                    }

                    #endregion

                    #region 系统管理免接口鉴权

                    var setting = await _settingService.FindSettingByName("IsAdminNotAuthentication");
                    if (setting != null && setting.Value.ToBool())
                    {
                        // loginUserInfo = await _apeContext.Cache.GetAsync<LoginUserInfo>(
                        //     GlobalConstants.CacheKey.OnlineKey +
                        //     _apeContext.HttpUser.JwtToken.ToMd5String16());
                        //if (loginUserInfo.IsAdmin) //不想查数据库就使用登录信息
                        var netUser = await _userService.QueryByIdAsync(_apeContext.HttpUser.Id);
                        if (netUser.IsAdmin)
                        {
                            context.Succeed(requirement);
                            return;
                        }
                    }

                    #endregion

                    #region 验证IP是否发生变化

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

                    #region 验证在线标识符 舍弃 你也可以保留

                    /*
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
*/

                    #endregion

                    #region 验证用户权限

                    var permissionVos = await _permissionService.GetPermissionVoAsync(_apeContext.HttpUser.Id);

                    if (permissionVos.Count != 0 && !requestPath.IsNullOrEmpty())
                    {
                        isMatchRole = permissionVos.Any(x =>
                            x.Url.Equals(requestPath, StringComparison.CurrentCultureIgnoreCase) &&
                            x.Method.Equals(requestMethod, StringComparison.CurrentCultureIgnoreCase));
                    }

                    #endregion

                    #region 验证权限标识符 舍弃 你也可以保留

                    /*
                    //权限url验证失败，再验证权接口是否有权限标识符
                    if (!isMatchRole)
                    {
                        try
                        {
                            var permissionRoles =
                                await _permissionService.GetPermissionRolesAsync(_apeContext.HttpUser.Id);
                            if (context.Resource.IsNotNull())
                            {
                                var endpointFeature = (IEndpointFeature)((DefaultHttpContext)context.Resource)
                                    .Features.FirstOrDefault(x =>
                                        x.Key.FullName == typeof(IEndpointFeature).FullName).Value;
                                var apeVoloAuthorize =
                                    endpointFeature.Endpoint?.Metadata.FirstOrDefault(x =>
                                        x.GetType() == typeof(ApeVoloAuthorizeAttribute)) as ApeVoloAuthorizeAttribute;
                                // .net core 3.1获取的方式
                                //ApeVoloAuthorizeAttribute apeVoloAuthorize = ((Endpoint)context.Resource).Metadata.FirstOrDefault(x => x.GetType() == typeof(ApeVoloAuthorizeAttribute)) as ApeVoloAuthorizeAttribute;

                                if (apeVoloAuthorize != null && apeVoloAuthorize.Roles.Any(role =>
                                        permissionRoles.Contains(role)))
                                {
                                    isMatchRole = true;
                                }
                            }
                        }
                        catch
                        {
                            // ignored
                        }
                    }
*/

                    #endregion

                    if (!isMatchRole)
                    {
                        context.Fail();
                        return;
                    }


                    context.Succeed(requirement);
                    return;
                }
            }

            context.Fail();
            return;
        }

        context.Fail();
    }
}
