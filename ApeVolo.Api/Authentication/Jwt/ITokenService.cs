using System.Threading.Tasks;
using ApeVolo.Common.WebApp;

namespace ApeVolo.Api.Authentication.Jwt;

public interface ITokenService
{
    /// <summary>
    /// Issue token
    /// </summary>
    /// <param name="loginUserInfo"></param>
    /// <returns></returns>
    Task<Token> IssueTokenAsync(LoginUserInfo loginUserInfo);
}
