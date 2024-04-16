using Ape.Volo.Business.Base;
using Ape.Volo.Common.WebApp;
using Ape.Volo.Entity.System;
using Ape.Volo.IBusiness.Interface.System;

namespace Ape.Volo.Business.System;

/// <summary>
/// Token黑名单
/// </summary>
public class TokenBlacklistService : BaseServices<TokenBlacklist>, ITokenBlacklistService
{
    public TokenBlacklistService(ApeContext apeContext) : base(apeContext)
    {
    }
}
