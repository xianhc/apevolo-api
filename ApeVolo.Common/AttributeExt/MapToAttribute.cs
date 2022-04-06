using System;

namespace ApeVolo.Common.AttributeExt;

[AttributeUsage(AttributeTargets.Class)]
public class MapToAttribute : Attribute
{
    public MapToAttribute(Type targetType)
    {
        TargetType = targetType;
    }

    public Type TargetType { get; }
}