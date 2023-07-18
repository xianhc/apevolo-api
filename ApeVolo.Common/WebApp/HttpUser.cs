using System;
using System.Linq;
using ApeVolo.Common.Global;
using Microsoft.AspNetCore.Http;

namespace ApeVolo.Common.WebApp;

public class HttpUser : IHttpUser
{
    private readonly HttpContext _httpContext;

    public HttpUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContext = httpContextAccessor?.HttpContext;
    }

    #region 登录ID

    /// <summary>
    /// 登录ID
    /// </summary>
    public long Id
    {
        get
        {
            if (IsAuthenticated)
            {
                var claim = _httpContext?.User.Claims.FirstOrDefault(s => s.Type == AuthConstants.JwtClaimTypes.Jti);
                return Convert.ToInt64(claim?.Value);
            }

            return default;
        }
    }

    #endregion

    #region 登录账号

    /// <summary>
    /// 登录账号
    /// </summary>
    public string Account
    {
        get
        {
            if (IsAuthenticated)
            {
                var claim = _httpContext?.User.Claims.FirstOrDefault(s => s.Type == AuthConstants.JwtClaimTypes.Name);
                return claim?.Value;
            }

            return string.Empty;
        }
    }

    #endregion

    #region jwt token

    /// <summary>
    /// jwt token
    /// </summary>
    /// <returns></returns>
    public string JwtToken
    {
        get
        {
            if (IsAuthenticated && _httpContext.Request.Headers.ContainsKey("Authorization"))
            {
                return _httpContext?.Request.Headers["Authorization"].ToString()
                    .Replace(AuthConstants.JwtTokenType, "").Trim();
            }

            return string.Empty;
        }
    }

    #endregion

    #region 是否已认证

    /// <summary>
    /// 是否已认证
    /// </summary>
    /// <returns></returns>
    public bool IsAuthenticated => _httpContext?.User.Identity?.IsAuthenticated ?? false;

    #endregion
}
