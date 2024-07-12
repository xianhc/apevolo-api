using System;

namespace Ape.Volo.Common.Attributes;

/// <summary>
/// 自定义校验特性
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class ApeVoloRequiredAttribute : Attribute
{
    /// <summary>
    /// 验证失败说明内容
    /// </summary>
    public string Message { get; set; }
}
