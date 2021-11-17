using System.Collections.Generic;
using System.Security.Claims;
using System.Linq;
using System.IdentityModel.Tokens.Jwt;

namespace ApeVolo.Common.WebApp
{
    public class CurrentUser : ICurrentUser
    {
        public string Name => HttpContextCore.CurrentHttpContext.User.Identity != null
            ? HttpContextCore.CurrentHttpContext.User.Identity.Name
            : null;

        public string Id => GetClaimValueByType("jti").FirstOrDefault();

        public bool IsAuthenticated()
        {
            return HttpContextCore.CurrentHttpContext.User.Identity is {IsAuthenticated: true};
        }


        public string GetToken()
        {
            return HttpContextCore.CurrentHttpContext.Request.Headers["Authorization"].ToString()
                .Replace("Bearer ", "");
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

        public IEnumerable<Claim> GetClaimsIdentity()
        {
            return HttpContextCore.CurrentHttpContext.User.Claims;
        }

        public List<string> GetClaimValueByType(string claimType)
        {
            var obj = (from item in GetClaimsIdentity()
                where item.Type == claimType
                select item.Value).ToList();

            return (from item in GetClaimsIdentity()
                where item.Type == claimType
                select item.Value).ToList();
        }
    }
}