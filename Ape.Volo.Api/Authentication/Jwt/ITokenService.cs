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
    /// <param name="refresh"></param>
    /// <returns></returns>
    Task<Token> IssueTokenAsync(LoginUserInfo loginUserInfo, bool refresh = false);

    Task<JwtSecurityToken> ReadJwtToken(string token);
}
