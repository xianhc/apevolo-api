using Ape.Volo.Common.Model;
using Ape.Volo.Entity.Base;
using SqlSugar;

namespace Ape.Volo.Entity.System;

/// <summary>
/// Token黑名单
/// </summary>
[SugarTable("sys_token_blacklist")]
public class TokenBlacklist : BaseEntity, ISoftDeletedEntity
{
    /// <summary>
    /// 令牌 登录token的MD5值
    /// </summary>
    public string AccessToken { get; set; }

    /// <summary>
    /// 是否删除
    /// </summary>
    public bool IsDeleted { get; set; }
}
