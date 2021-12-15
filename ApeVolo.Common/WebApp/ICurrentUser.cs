using System.Collections.Generic;
using System.Security.Claims;

namespace ApeVolo.Common.WebApp
{
    /// <summary>
    /// 当前用户
    /// </summary>
    public interface ICurrentUser
    {
        string Name { get; }
        long Id { get; }
        bool IsAuthenticated();
        IEnumerable<Claim> GetClaimsIdentity();
        List<string> GetClaimValueByType(string claimType);
        string GetToken();
        List<string> GetUserInfoFromToken(string claimType);
    }
}
