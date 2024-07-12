using System;

namespace Ape.Volo.Common.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class MapFromAttribute : Attribute
{
    public MapFromAttribute(Type fromType)
    {
        FromType = fromType;
    }

    public Type FromType { get; }
}
