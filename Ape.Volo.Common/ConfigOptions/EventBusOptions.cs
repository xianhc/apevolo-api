using Ape.Volo.Common.Attributes;

namespace Ape.Volo.Common.ConfigOptions;

/// <summary>
/// 事件总线配置
/// </summary>
[OptionsSettings]
public class EventBusOptions
{
    /// <summary>
    /// 是否启用
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// 订阅名称
    /// </summary>
    public string SubscriptionClientName { get; set; }
}
