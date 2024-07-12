using System;

namespace Ape.Volo.Common.Attributes;

/// <summary>
/// 初始种子数据特性，标有该特性的实体表示程序则生成数据库表数据
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class InitTableAttribute : Attribute
{
    public InitTableAttribute(Type sourceType)
    {
        SourceType = sourceType;
    }

    /// <summary>
    /// 源对象
    /// </summary>
    public Type SourceType { get; }
}
