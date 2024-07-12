using System;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Ape.Volo.Common;
using Ape.Volo.Common.Extensions;
using Ape.Volo.Common.Global;
using Ape.Volo.Common.Model;
using Ape.Volo.Common.WebApp;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Ape.Volo.Api.Authentication.Jwt;

public class ApiResponseHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public ApiResponseHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger,
        UrlEncoder encoder) :
        base(options, logger, encoder)
    {
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
            Message = "抱歉，您无权访问该接口",
            Path = App.HttpContext?.Request.Path.Value?.ToLower()
        }.ToJson());
    }

    protected override async Task HandleForbiddenAsync(AuthenticationProperties properties)
    {
        var loginUserInfo = await App.Cache.GetAsync<LoginUserInfo>(
            GlobalConstants.CachePrefix.OnlineKey +
            App.HttpUser.JwtToken.ToMd5String16());
        if (loginUserInfo.IsNull())
        {
            Response.ContentType = "application/json";
            Response.StatusCode = StatusCodes.Status401Unauthorized;
            await Response.WriteAsync(new ActionResultVm
            {
                Status = StatusCodes.Status401Unauthorized,
                ActionError = new ActionError(),
                Message = "抱歉，您无权访问该接口",
                Path = App.HttpContext?.Request.Path.Value?.ToLower()
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
                Message = "抱歉，您访问权限等级不够",
                Path = App.HttpContext?.Request.Path.Value?.ToLower()
            }.ToJson());
        }
    }
}
