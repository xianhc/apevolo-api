namespace ApeVolo.EventBus.Abstractions;

/// <summary>
/// 动态集成事件处理器接口
/// </summary>
public interface IDynamicIntegrationEventHandler
{
    Task Handle(dynamic eventData);
}