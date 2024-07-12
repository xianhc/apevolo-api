using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Ape.Volo.Common;
using Ape.Volo.Common.ConfigOptions;
using Ape.Volo.Common.Extensions;
using Ape.Volo.Common.Global;
using Ape.Volo.Common.WebApp;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Ape.Volo.Api.Authentication.Jwt;

public class TokenService : ITokenService
{
    // private readonly JwtAuthOptions _jwtOptionses;
    //
    //
    // public TokenService(JwtAuthOptions jwtAuthOptions)
    // {
    //     _jwtOptionses = jwtAuthOptions;
    // }

    public async Task<Token> IssueTokenAsync(LoginUserInfo loginUserInfo, bool refresh = false)
    {
        if (loginUserInfo == null)
            throw new ArgumentNullException(nameof(loginUserInfo));

        var jwtAuthOptions = App.GetOptions<JwtAuthOptions>();
        var signinCredentials =
            new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtAuthOptions.SecurityKey)),
                SecurityAlgorithms.HmacSha256);
        var nowTime = DateTime.Now;
        var cls = new List<Claim>
        {
            new(AuthConstants.JwtClaimTypes.Jti, loginUserInfo.UserId.ToString()),
            new(AuthConstants.JwtClaimTypes.Name, loginUserInfo.Account),
            new(AuthConstants.JwtClaimTypes.TenantId, loginUserInfo.TenantId.ToString()),
            new(AuthConstants.JwtClaimTypes.DeptId, loginUserInfo.DeptId.ToString()),
            new(AuthConstants.JwtClaimTypes.Iat, nowTime.ToUnixTimeStampSecond().ToString()),
            new(AuthConstants.JwtClaimTypes.Ip, loginUserInfo.Ip)
        };
        var identity = new ClaimsIdentity(AuthConstants.JwtTokenType);
        identity.AddClaims(cls);


        var tokeOptions = new JwtSecurityToken(
            issuer: jwtAuthOptions.Issuer,
            audience: jwtAuthOptions.Audience,
            claims: cls,
            notBefore: nowTime,
            expires: nowTime.AddHours(jwtAuthOptions.Expires),
            signingCredentials: signinCredentials
        );


        var token = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
        if (refresh)
        {
            return await Task.FromResult(new Token()
            {
                Expires = jwtAuthOptions.Expires * 3600,
                TokenType = AuthConstants.JwtTokenType,
                RefreshToken = token,
            });
        }

        return await Task.FromResult(new Token()
        {
            AccessToken = token,
            Expires = jwtAuthOptions.Expires * 3600,
            TokenType = AuthConstants.JwtTokenType,
            RefreshToken = "",
            RefreshTokenExpires = jwtAuthOptions.RefreshTokenExpires * 3600
        });
    }


    public async Task<JwtSecurityToken> ReadJwtToken(string token)
    {
        token = token.Replace(AuthConstants.JwtTokenType, "").Trim();
        var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        if (jwtSecurityTokenHandler.CanReadToken(token))
        {
            var jwtAuthOptions = App.GetOptions<JwtAuthOptions>();
            var signinCredentials =
                new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtAuthOptions.SecurityKey)),
                    SecurityAlgorithms.HmacSha256);
            JwtSecurityToken jwtSecurityToken = jwtSecurityTokenHandler.ReadJwtToken(token);
            var rawSignature = JwtTokenUtilities.CreateEncodedSignature(
                jwtSecurityToken.RawHeader + "." + jwtSecurityToken.RawPayload,
                signinCredentials);
            if (jwtSecurityToken.RawSignature == rawSignature)
            {
                return await Task.FromResult(jwtSecurityToken);
            }
        }

        return null;
    }
}
