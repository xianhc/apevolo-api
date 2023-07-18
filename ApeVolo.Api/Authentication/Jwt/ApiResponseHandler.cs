using System;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using ApeVolo.Common.Caches.Redis.Service;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Global;
using ApeVolo.Common.Model;
using ApeVolo.Common.Resources;
using ApeVolo.Common.WebApp;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ApeVolo.Api.Authentication.Jwt;

public class ApiResponseHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly ApeContext _apeContext;

    public ApiResponseHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger,
        UrlEncoder encoder, ISystemClock clock, ApeContext apeContext) :
        base(options, logger, encoder, clock)
    {
        _apeContext = apeContext;
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
            ActionError = new ActionError(),
            Message = Localized.Get("HttpUnauthorized"),
            Path = HttpContextCore.CurrentHttpContext.Request.Path.Value?.ToLower()
        }.ToJson());
    }

    protected override async Task HandleForbiddenAsync(AuthenticationProperties properties)
    {
        var loginUserInfo = await _apeContext.RedisCache.GetCacheAsync<LoginUserInfo>(
            GlobalConstants.CacheKey.OnlineKey +
            _apeContext.HttpUser.JwtToken.ToMd5String16());
        if (loginUserInfo.IsNull())
        {
            Response.ContentType = "application/json";
            Response.StatusCode = StatusCodes.Status401Unauthorized;
            await Response.WriteAsync(new ActionResultVm
            {
                Status = StatusCodes.Status401Unauthorized,
                ActionError = new ActionError(),
                Message = Localized.Get("HttpUnauthorized"),
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
                ActionError = new ActionError(),
                Message = Localized.Get("HttpForbidden"),
                Path = HttpContextCore.CurrentHttpContext.Request.Path.Value?.ToLower()
            }.ToJson());
        }
    }
}
