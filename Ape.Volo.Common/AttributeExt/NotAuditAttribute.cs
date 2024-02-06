using System;

namespace Ape.Volo.Common.AttributeExt;

/// <summary>
/// 表示不需要记录审计日志
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class NotAuditAttribute : Attribute;
