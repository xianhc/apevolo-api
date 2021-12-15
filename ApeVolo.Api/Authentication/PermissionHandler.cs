using System;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ApeVolo.Common.AttributeExt;
using ApeVolo.Common.Caches.Redis.Service;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Global;
using ApeVolo.Common.WebApp;
using ApeVolo.Entity.Do.Other;
using ApeVolo.IBusiness.Interface.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;

namespace ApeVolo.Api.Authentication
{
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
        private readonly IHttpContextAccessor _accessor;
        private readonly ICurrentUser _currentUser;
        private readonly ISettingService _settingService;
        private readonly IRedisCacheService _redisCacheService;

        /// <summary>
        /// 构造函数注入
        /// </summary>
        /// <param name="schemes"></param>
        /// <param name="accessor"></param>
        /// <param name="permissionService"></param>
        /// <param name="currentUser"></param>
        /// <param name="settingService"></param>
        /// <param name="redisCacheService"></param>
        public PermissionHandler(IAuthenticationSchemeProvider schemes, IHttpContextAccessor accessor,
            IPermissionService permissionService, ICurrentUser currentUser, ISettingService settingService,
            IRedisCacheService redisCacheService)
        {
            _accessor = accessor;
            Schemes = schemes;
            _permissionService = permissionService;
            _currentUser = currentUser;
            _settingService = settingService;
            _redisCacheService = redisCacheService;
        }

        // 重写异步处理程序
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            var isMatchRole = false;
            var onlineUser = await _redisCacheService.GetCacheAsync<OnlineUser>(RedisKey.OnlineKey +
                _currentUser.GetToken()
                    .ToHmacsha256String(
                        AppSettings.GetValue(
                            "HmacSecret")));
            if (onlineUser.IsNotNull())
            {
                var httpContext = _accessor.HttpContext;
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
                        if (result.Principal != null)
                        {
                            httpContext.User = result.Principal;
                            try
                            {
                                //在线特性，接口拥有ApeVoloOnlineAttribute 直接放行
                                if (context.Resource.IsNotNull())
                                {
                                    var endpointFeature = (IEndpointFeature) ((DefaultHttpContext) context.Resource)
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

                            //验证权限 需要保持用户最新权限信息可以刷新在线用户OnlineUser.CurrentPermission
                            if ((await _settingService.FindSettingByName("IsRealTimeAuthentication")).Value.ToBool())
                            {
                                //用户权限点 路由
                                var role = _currentUser.GetClaimValueByType(ClaimTypes.Role);
                                var permissionList = await _permissionService.QueryUserPermissionAsync(_currentUser.Id);
                                onlineUser.currentPermission.Urls.AddRange(permissionList.Select(s => s.Permission)
                                    .Where(s => !s.IsNullOrEmpty()));
                                onlineUser.currentPermission.Urls.AddRange(role.Select(r => r));
                                onlineUser.currentPermission.Urls.AddRange(permissionList.Select(s => s.LinkUrl)
                                    .Where(s => !s.IsNullOrEmpty()));
                            }

                            //验证url
                            foreach (var url in onlineUser.currentPermission.Urls)
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
                                        var endpointFeature = (IEndpointFeature) ((DefaultHttpContext) context.Resource)
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
                                                onlineUser.currentPermission.Roles.Contains(role)))
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

                            if (httpContext.User.Claims.SingleOrDefault(s => s.Type == ClaimTypes.Expiration)?.Value !=
                                null && DateTime.Parse(httpContext.User.Claims
                                    .SingleOrDefault(s => s.Type == ClaimTypes.Expiration)?.Value ?? string.Empty) >=
                                DateTime.Now)
                            {
                                context.Succeed(requirement);
                                return;
                            }
                        }
                    }

                    //判断没有登录时，是否访问登录的url,并且是Post请求，并且是form表单提交类型，否则为失败
                    if (questUrl != null &&
                        !questUrl.Equals(requirement.LoginPath.ToLower(), StringComparison.Ordinal) &&
                        (!httpContext.Request.Method.Equals("POST") || !httpContext.Request.HasFormContentType))
                    {
                        context.Fail();
                        return;
                    }
                }
            }

            context.Fail();
        }
    }
}