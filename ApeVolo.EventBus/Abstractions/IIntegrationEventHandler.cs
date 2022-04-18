using ApeVolo.EventBus.Events;

namespace ApeVolo.EventBus.Abstractions;

/// <summary>
/// 集成事件处理器接口
/// </summary>
/// <typeparam name="TIntegrationEvent"></typeparam>
public interface IIntegrationEventHandler<in TIntegrationEvent> : IIntegrationEventHandler
    where TIntegrationEvent : IntegrationEvent
{
    Task Handle(TIntegrationEvent @event);
}

/// <summary>
/// 集成事件处理器
/// </summary>
public interface IIntegrationEventHandler
{
}