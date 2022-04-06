using System;

namespace ApeVolo.Common.AttributeExt;

/// <summary>
/// dto映射属性
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class AutoMappingAttribute : Attribute
{
    public AutoMappingAttribute(Type sourceType, Type targetType)
    {
        SourceType = sourceType;
        TargetType = targetType;
    }

    /// <summary>
    /// 源对象
    /// </summary>
    public Type SourceType { get; }

    /// <summary>
    /// 目标对象
    /// </summary>
    public Type TargetType { get; }
}