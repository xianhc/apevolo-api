using System;

namespace ApeVolo.Common.AttributeExt
{
    public class MapFromAttribute : Attribute
    {
        public MapFromAttribute(Type fromType)
        {
            FromType = fromType;
        }
        public Type FromType { get; }
    }
}
