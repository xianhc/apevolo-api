using System;

namespace ApeVolo.Common.AttributeExt;

/// <summary>
/// 自定义鉴权特性，在线则可通行
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class ApeVoloOnlineAttribute : Attribute
{
}