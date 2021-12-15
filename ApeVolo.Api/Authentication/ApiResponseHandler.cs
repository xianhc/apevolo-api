using System;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using ApeVolo.Common.Caches.Redis.Service;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Global;
using ApeVolo.Common.Model;
using ApeVolo.Common.WebApp;
using ApeVolo.Entity.Do.Other;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ApeVolo.Api.Authentication
{
    public class ApiResponseHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly ICurrentUser _currentUser;
        private readonly IRedisCacheService _redisCacheService;

        public ApiResponseHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger,
            UrlEncoder encoder, ISystemClock clock, ICurrentUser currentUser, IRedisCacheService redisCacheService) :
            base(options, logger, encoder, clock)
        {
            _currentUser = currentUser;
            _redisCacheService = redisCacheService;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            throw new NotImplementedException();
        }

        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.ContentType = "application/json";
            Response.StatusCode = StatusCodes.Status401Unauthorized;
            await Response.WriteAsync(new ActionResultVm
            {
                Status = StatusCodes.Status401Unauthorized,
                Error = "Unauthorized",
                Message = "抱歉，您无权访问该接口!!!",
                Path = HttpContextCore.CurrentHttpContext.Request.Path.Value?.ToLower()
            }.ToJson());
        }

        protected override async Task HandleForbiddenAsync(AuthenticationProperties properties)
        {
            var onlineUser = await _redisCacheService.GetCacheAsync<OnlineUser>(RedisKey.OnlineKey +
                _currentUser.GetToken()
                    .ToHmacsha256String(
                        AppSettings.GetValue("HmacSecret")));
            if (onlineUser.IsNull())
            {
                Response.ContentType = "application/json";
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                await Response.WriteAsync(new ActionResultVm
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Error = "Unauthorized",
                    Message = "抱歉，您无权访问该接口!!!",
                    Path = HttpContextCore.CurrentHttpContext.Request.Path.Value?.ToLower()
                }.ToJson());
            }
            else
            {
                Response.ContentType = "application/json";
                Response.StatusCode = StatusCodes.Status403Forbidden;
                await Response.WriteAsync(new ActionResultVm
                {
                    Status = StatusCodes.Status403Forbidden,
                    Error = "Forbidden",
                    Message = "抱歉，您访问权限等级不够!!!",
                    Path = HttpContextCore.CurrentHttpContext.Request.Path.Value?.ToLower()
                }.ToJson());
            }
        }
    }
}