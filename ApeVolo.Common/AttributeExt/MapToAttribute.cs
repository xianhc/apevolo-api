using System;

namespace ApeVolo.Common.AttributeExt;

public class MapToAttribute : Attribute
{
    public MapToAttribute(Type targetType)
    {
        TargetType = targetType;
    }

    public Type TargetType { get; }
}