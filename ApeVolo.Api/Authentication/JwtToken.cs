using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ApeVolo.Api.Authentication;

/// <summary>
/// JWTToken生成类
/// </summary>
public static class JwtToken
{
    /// <summary>
    /// 获取基于JWT的Token
    /// </summary>
    /// <param name="claims">需要在登陆的时候配置</param>
    /// <param name="permissionRequirement">在startup中定义的参数</param>
    /// <returns></returns>
    public static dynamic BuildJwtToken(Claim[] claims, PermissionRequirement permissionRequirement)
    {
        var now = DateTime.Now;
        // 实例化JwtSecurityToken
        var jwt = new JwtSecurityToken(
            issuer: permissionRequirement.Issuer,
            audience: permissionRequirement.Audience,
            claims: claims,
            notBefore: now,
            expires: now.Add(permissionRequirement.Expiration),
            signingCredentials: permissionRequirement.SigningCredentials
        );
        var token = new JwtSecurityTokenHandler().WriteToken(jwt);
        return "Bearer " + token;
    }
}