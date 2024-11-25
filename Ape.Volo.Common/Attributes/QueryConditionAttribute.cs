using System;

namespace Ape.Volo.Common.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class QueryConditionAttribute : Attribute
{
    public QueryConditionType ConditionType { get; }

    public QueryConditionAttribute(QueryConditionType conditionType)
    {
        ConditionType = conditionType;
    }
}

public enum QueryConditionType
{
    Equal,
    Like,
    // 可以添加其他操作符类型
}
