using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Ape.Volo.Common.WebApp;

namespace Ape.Volo.Api.Authentication.Jwt;

public interface ITokenService
{
    /// <summary>
    /// Issue token
    /// </summary>
    /// <param name="loginUserInfo"></param>
    /// <returns></returns>
    Task<Token> IssueTokenAsync(LoginUserInfo loginUserInfo);

    Task<JwtSecurityToken> ReadJwtToken(string token);
}
