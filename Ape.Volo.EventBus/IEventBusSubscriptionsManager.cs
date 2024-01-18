using Ape.Volo.EventBus.Abstractions;
using Ape.Volo.EventBus.Events;

namespace Ape.Volo.EventBus;

/// <summary>
/// 事件总线订阅管理器接口
/// </summary>
public interface IEventBusSubscriptionsManager
{
    bool IsEmpty { get; }
    event EventHandler<string> OnEventRemoved;

    /// <summary>
    /// 添加动态订阅
    /// </summary>
    /// <typeparam name="TH"></typeparam>
    /// <param name="eventName"></param>
    void AddDynamicSubscription<TH>(string eventName)
        where TH : IDynamicIntegrationEventHandler;

    /// <summary>
    /// 添加订阅
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TH"></typeparam>
    void AddSubscription<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>;

    /// <summary>
    /// 删除订阅
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TH"></typeparam>
    void RemoveSubscription<T, TH>()
        where TH : IIntegrationEventHandler<T>
        where T : IntegrationEvent;

    /// <summary>
    /// 移除动态订阅
    /// </summary>
    /// <typeparam name="TH"></typeparam>
    /// <param name="eventName"></param>
    void RemoveDynamicSubscription<TH>(string eventName)
        where TH : IDynamicIntegrationEventHandler;

    /// <summary>
    /// 已订阅事件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    bool HasSubscriptionsForEvent<T>() where T : IntegrationEvent;

    /// <summary>
    /// 已订阅事件
    /// </summary>
    /// <param name="eventName"></param>
    /// <returns></returns>
    bool HasSubscriptionsForEvent(string eventName);

    /// <summary>
    /// 按名称获取事件类型
    /// </summary>
    /// <param name="eventName"></param>
    /// <returns></returns>
    Type GetEventTypeByName(string eventName);

    /// <summary>
    /// 清空
    /// </summary>
    void Clear();

    /// <summary>
    /// 获取事件处理程序
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    IEnumerable<InMemoryEventBusSubscriptionsManager.SubscriptionInfo> GetHandlersForEvent<T>()
        where T : IntegrationEvent;

    /// <summary>
    /// 获取事件处理程序
    /// </summary>
    /// <param name="eventName"></param>
    /// <returns></returns>
    IEnumerable<InMemoryEventBusSubscriptionsManager.SubscriptionInfo> GetHandlersForEvent(string eventName);

    /// <summary>
    /// 获取事件密钥
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    string GetEventKey<T>();
}
