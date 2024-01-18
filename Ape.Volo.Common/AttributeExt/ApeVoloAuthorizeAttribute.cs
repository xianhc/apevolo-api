using System;

namespace Ape.Volo.Common.AttributeExt;

/// <summary>
/// 自定义鉴权特性
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class ApeVoloAuthorizeAttribute : Attribute
{
    public ApeVoloAuthorizeAttribute(string[] roles)
    {
        Roles = roles;
    }

    /// <summary>
    /// 角色代码
    /// </summary>
    public string[] Roles { get; }
}
