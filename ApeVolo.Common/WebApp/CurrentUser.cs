using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using ApeVolo.Common.Extention;
using Microsoft.AspNetCore.Http;

namespace ApeVolo.Common.WebApp;

public class CurrentUser : ICurrentUser
{
    private readonly HttpContext _httpContext;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContext = httpContextAccessor.HttpContext;
    }

    /// <summary>
    /// 当前登录用户名称
    /// </summary>
    public string Name => _httpContext.User.Identity?.Name;

    /// <summary>
    /// 当前登录用户ID
    /// </summary>
    public long Id => GetClaimValueByType("jti").FirstOrDefault().ToLong();

    /// <summary>
    /// 请求携带的Token
    /// </summary>
    /// <returns></returns>
    public string GetToken()
    {
        return _httpContext.Request.Headers["Authorization"].ToString()
            .Replace("Bearer ", "");
    }

    /// <summary>
    /// 是否已认证
    /// </summary>
    /// <returns></returns>
    public bool IsAuthenticated()
    {
        return _httpContext.User.Identity is { IsAuthenticated: true };
    }

    /// <summary>
    /// 获取用户身份权限
    /// </summary>
    /// <returns></returns>
    public IEnumerable<Claim> GetClaimsIdentity()
    {
        return _httpContext.User.Claims;
    }

    /// <summary>
    /// 根据权限类型获取详细权限
    /// </summary>
    /// <param name="claimType"></param>
    /// <returns></returns>
    public List<string> GetClaimValueByType(string claimType)
    {
        return (from item in GetClaimsIdentity()
            where item.Type == claimType
            select item.Value).ToList();
    }

    public List<string> GetUserInfoFromToken(string claimType)
    {
        var jwtHandler = new JwtSecurityTokenHandler();
        if (!string.IsNullOrEmpty(GetToken()))
        {
            JwtSecurityToken jwtToken = jwtHandler.ReadJwtToken(GetToken());

            return (from item in jwtToken.Claims
                where item.Type == claimType
                select item.Value).ToList();
        }

        return new List<string>();
    }
}