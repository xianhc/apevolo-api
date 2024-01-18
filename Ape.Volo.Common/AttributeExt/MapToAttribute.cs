using System;

namespace Ape.Volo.Common.AttributeExt;

[AttributeUsage(AttributeTargets.Class)]
public class MapToAttribute : Attribute
{
    public MapToAttribute(Type targetType)
    {
        TargetType = targetType;
    }

    public Type TargetType { get; }
}
