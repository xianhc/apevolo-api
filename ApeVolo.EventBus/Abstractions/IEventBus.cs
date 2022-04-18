using ApeVolo.Common.DI;
using ApeVolo.EventBus.Events;

namespace ApeVolo.EventBus.Abstractions;

/// <summary>
/// 事件总线接口
/// https://www.cnblogs.com/hudean/p/15330488.html
/// </summary>
public interface IEventBus
{
    /// <summary>
    /// 发布
    /// </summary>
    /// <param name="event"></param>
    void Publish(IntegrationEvent @event);

    /// <summary>
    /// 订阅
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TH"></typeparam>
    void Subscribe<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>;

    /// <summary>
    /// 取消订阅
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TH"></typeparam>
    void Unsubscribe<T, TH>()
        where TH : IIntegrationEventHandler<T>
        where T : IntegrationEvent;


    /// <summary>
    /// 动态订阅
    /// </summary>
    /// <typeparam name="TH"></typeparam>
    /// <param name="eventName"></param>
    void SubscribeDynamic<TH>(string eventName)
        where TH : IDynamicIntegrationEventHandler;

    /// <summary>
    /// 取消动态订阅
    /// </summary>
    /// <typeparam name="TH"></typeparam>
    /// <param name="eventName"></param>
    void UnsubscribeDynamic<TH>(string eventName)
        where TH : IDynamicIntegrationEventHandler;
}