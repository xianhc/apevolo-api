using System;

namespace Ape.Volo.Common.AttributeExt;

/// <summary>
/// 事务特性 AOP拦截使用
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class UseTranAttribute : Attribute
{
}
