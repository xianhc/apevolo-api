using System.Collections.Generic;

namespace ApeVolo.Entity.Do.Other;

/// <summary>
/// 在线用户授权信息
/// </summary>
public class CurrentPermission
{
    /// <summary>
    /// URL
    /// </summary>
    public List<string> Urls { get; set; }

    /// <summary>
    /// 角色标识
    /// </summary>
    public List<string> Roles { get; set; }
}