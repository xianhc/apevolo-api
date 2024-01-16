using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ApeVolo.Common.ConfigOptions;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Global;
using ApeVolo.Common.WebApp;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace ApeVolo.Api.Authentication.Jwt;

public class TokenService : ITokenService
{
    private readonly JwtAuthOption _jwtOptions;


    public TokenService(IOptionsMonitor<Configs> configs)
    {
        _jwtOptions = (configs?.CurrentValue ?? new Configs()).JwtAuthOptions;
    }

    public async Task<Token> IssueTokenAsync(LoginUserInfo loginUserInfo)
    {
        if (loginUserInfo == null)
            throw new ArgumentNullException(nameof(loginUserInfo));

        var signinCredentials =
            new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecurityKey)),
                SecurityAlgorithms.HmacSha256);
        var nowTime = DateTime.Now;
        var cls = new List<Claim>
        {
            new(AuthConstants.JwtClaimTypes.Jti, loginUserInfo.UserId.ToString()),
            new(AuthConstants.JwtClaimTypes.Name, loginUserInfo.Account),
            new(AuthConstants.JwtClaimTypes.Iat, nowTime.ToUnixTimeStampSecond().ToString()),
            new(AuthConstants.JwtClaimTypes.Ip, loginUserInfo.Ip)
        };
        var identity = new ClaimsIdentity(AuthConstants.JwtTokenType);
        identity.AddClaims(cls);


        var tokeOptions = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: cls,
            notBefore: nowTime,
            expires: nowTime.AddSeconds(_jwtOptions.Expires),
            signingCredentials: signinCredentials
        );


        var token = new JwtSecurityTokenHandler().WriteToken(tokeOptions);

        return await Task.FromResult(new Token()
        {
            AccessToken = token,
            Expires = _jwtOptions.Expires,
            TokenType = AuthConstants.JwtTokenType,
            RefreshToken = "",
            RefreshTokenExpires = _jwtOptions.RefreshTokenExpires
        });
    }


    public async Task<JwtSecurityToken> ReadJwtToken(string token)
    {
        token = token.Replace(AuthConstants.JwtTokenType, "").Trim();
        var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        if (jwtSecurityTokenHandler.CanReadToken(token))
        {
            var signinCredentials =
                new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecurityKey)),
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
