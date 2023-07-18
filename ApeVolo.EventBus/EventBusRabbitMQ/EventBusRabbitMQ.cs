using System.Net.Sockets;
using System.Text;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Helper.Serilog;
using ApeVolo.EventBus.Abstractions;
using ApeVolo.EventBus.Events;
using Autofac;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using Serilog;

namespace ApeVolo.EventBus.EventBusRabbitMQ;

/// <summary>
/// RabbitMQ消息队列事件
/// </summary>
public class EventBusRabbitMQ : IEventBus, IDisposable
{
    #region 字段

    private static readonly ILogger Logger = SerilogManager.GetLogger(typeof(EventBusRabbitMQ));
    const string BrokerName = "apevolo_event_bus";
    const string AutofacScopeName = "apevolo_event_bus";
    private readonly IRabbitMQPersistentConnection _persistentConnection;
    private readonly IEventBusSubscriptionsManager _subsManager;
    private readonly ILifetimeScope _autofac;
    private readonly int _retryCount;
    private IModel _consumerChannel;

    private string _subscriptionClientName;

    //后面把AutoFac的改成.net core 自带的生命周期
    private readonly IServiceProvider _serviceProvider;

    #endregion

    #region 构造函数

    public EventBusRabbitMQ(IServiceProvider serviceProvider, IRabbitMQPersistentConnection persistentConnection,
        ILifetimeScope autofac, string subscriptionClientName, IEventBusSubscriptionsManager? subsManager,
        int retryCount = 5)
    {
        _persistentConnection = persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
        _subsManager = subsManager ?? new InMemoryEventBusSubscriptionsManager();
        _subscriptionClientName = subscriptionClientName;
        _consumerChannel = CreateConsumerChannel();
        _autofac = autofac;
        _retryCount = retryCount;
        _subsManager.OnEventRemoved += SubsManager_OnEventRemoved;
        _serviceProvider = serviceProvider;
    }

    #endregion

    #region 发布与订阅

    /// <summary>
    /// 发布
    /// </summary>
    /// <param name="event"></param>
    public void Publish(IntegrationEvent @event)
    {
        if (!_persistentConnection.IsConnected)
        {
            _persistentConnection.TryConnect();
        }

        var policy = Policy.Handle<BrokerUnreachableException>()
            .Or<SocketException>()
            .WaitAndRetry(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (ex, time) =>
                {
                    Logger.Warning(
                        $"Could not publish event: {@event.Id} after {time.TotalSeconds:n1}s ({ex.Message})");
                });
        var eventName = @event.GetType().Name;

        Logger.Information($"Creating RabbitMQ channel to publish event: {@event.Id} ({eventName})");
        using (var channel = _persistentConnection.CreateModel())
        {
            Logger.Information($"Declaring RabbitMQ exchange to publish event: {@event.Id}");
            channel.ExchangeDeclare(exchange: BrokerName, type: "direct");

            var message = JsonConvert.SerializeObject(@event);
            var body = Encoding.UTF8.GetBytes(message);

            policy.Execute(() =>
            {
                var properties = channel.CreateBasicProperties();
                properties.DeliveryMode = 2; // persistent

                Logger.Information($"Publishing event to RabbitMQ: {@event.Id}");
                channel.BasicPublish(
                    exchange: BrokerName,
                    routingKey: eventName,
                    mandatory: true,
                    basicProperties: properties,
                    body: body);
            });
        }
    }

    /// <summary>
    /// 订阅
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TH"></typeparam>
    public void Subscribe<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>
    {
        var eventName = _subsManager.GetEventKey<T>();
        DoInternalSubscription(eventName);

        Logger.Information($"Subscribing to event {eventName} with {typeof(TH).GetGenericTypeName()}");
        _subsManager.AddSubscription<T, TH>();
        StartBasicConsume();
    }


    /// <summary>
    /// 取消订阅
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TH"></typeparam>
    public void Unsubscribe<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>
    {
        var eventName = _subsManager.GetEventKey<T>();

        Logger.Information($"Unsubscribing from event {eventName}");
        _subsManager.RemoveSubscription<T, TH>();
    }


    /// <summary>
    /// 动态订阅
    /// </summary>
    /// <typeparam name="TH"></typeparam>
    /// <param name="eventName"></param>
    public void SubscribeDynamic<TH>(string eventName)
        where TH : IDynamicIntegrationEventHandler
    {
        Logger.Information($"Subscribing to dynamic event {eventName} with {typeof(TH).GetGenericTypeName()}");

        DoInternalSubscription(eventName);
        _subsManager.AddDynamicSubscription<TH>(eventName);
        StartBasicConsume();
    }

    /// <summary>
    /// 取消动态订阅
    /// </summary>
    /// <param name="eventName"></param>
    /// <typeparam name="TH"></typeparam>
    public void UnsubscribeDynamic<TH>(string eventName)
        where TH : IDynamicIntegrationEventHandler
    {
        _subsManager.RemoveDynamicSubscription<TH>(eventName);
    }

    #endregion

    #region 订阅事件

    /// <summary>
    /// 订阅管理器删除事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="eventName"></param>
    private void SubsManager_OnEventRemoved(object sender, string eventName)
    {
        if (!_persistentConnection.IsConnected)
        {
            _persistentConnection.TryConnect();
        }

        using (var channel = _persistentConnection.CreateModel())
        {
            channel.QueueUnbind(queue: _subscriptionClientName,
                exchange: BrokerName,
                routingKey: eventName);

            if (_subsManager.IsEmpty)
            {
                _subscriptionClientName = string.Empty;
                _consumerChannel.Close();
            }
        }
    }


    /// <summary>
    /// 做内部订阅
    /// </summary>
    /// <param name="eventName"></param>
    private void DoInternalSubscription(string eventName)
    {
        var containsKey = _subsManager.HasSubscriptionsForEvent(eventName);
        if (!containsKey)
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            _consumerChannel.QueueBind(queue: _subscriptionClientName,
                exchange: BrokerName,
                routingKey: eventName);
        }
    }

    #endregion

    #region 消费

    /// <summary>
    /// 开始基本消费
    /// </summary>
    private void StartBasicConsume()
    {
        Logger.Information("Starting RabbitMQ basic consume");
        if (_consumerChannel != null)
        {
            var consumer = new AsyncEventingBasicConsumer(_consumerChannel);

            consumer.Received += Consumer_Received;

            _consumerChannel.BasicConsume(
                queue: _subscriptionClientName,
                autoAck: false,
                consumer: consumer);
        }
        else
        {
            Logger.Error("StartBasicConsume can't call on _consumerChannel == null");
        }
    }

    /// <summary>
    /// 消费者收到消息
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="eventArgs"></param>
    /// <returns></returns>
    private async Task Consumer_Received(object sender, BasicDeliverEventArgs eventArgs)
    {
        var eventName = eventArgs.RoutingKey;
        var message = Encoding.UTF8.GetString(eventArgs.Body.Span);

        try
        {
            if (message.ToLowerInvariant().Contains("throw-fake-exception"))
            {
                throw new InvalidOperationException($"Fake exception requested: \"{message}\"");
            }

            await ProcessEvent(eventName, message);
            //await ProcessEventByNetCore(eventName, message);
        }
        catch (Exception ex)
        {
            Logger.Error($"ERROR Processing message {message} ex:{ex.Message}");
        }

        // Even on exception we take the message off the queue.
        // in a REAL WORLD app this should be handled with a Dead Letter Exchange (DLX). 
        // For more information see: https://www.rabbitmq.com/dlx.html
        _consumerChannel.BasicAck(eventArgs.DeliveryTag, multiple: false);
    }

    #endregion

    #region 创建消费者频道创建消费者频道

    /// <summary>
    /// 创建消费者频道
    /// </summary>
    /// <returns></returns>
    private IModel CreateConsumerChannel()
    {
        if (!_persistentConnection.IsConnected)
        {
            _persistentConnection.TryConnect();
        }

        Logger.Information("Creating RabbitMQ consumer channel");

        var channel = _persistentConnection.CreateModel();

        channel.ExchangeDeclare(exchange: BrokerName,
            type: "direct");

        channel.QueueDeclare(queue: _subscriptionClientName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        channel.CallbackException += (sender, ea) =>
        {
            Logger.Warning("Recreating RabbitMQ consumer channel");
            _consumerChannel.Dispose();
            _consumerChannel = CreateConsumerChannel();
            StartBasicConsume();
        };

        return channel;
    }

    #endregion

    #region 进程事件

    /// <summary>
    /// 进程事件（使用autofac）推荐
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    private async Task ProcessEvent(string eventName, string message)
    {
        Logger.Information($"Processing RabbitMQ event: {eventName}");
        if (_subsManager.HasSubscriptionsForEvent(eventName))
        {
            using (var scope = _autofac.BeginLifetimeScope(AutofacScopeName))
            {
                var subscriptions = _subsManager.GetHandlersForEvent(eventName);
                foreach (var subscription in subscriptions)
                {
                    if (subscription.IsDynamic)
                    {
                        var handler =
                            scope.ResolveOptional(subscription.HandlerType) as IDynamicIntegrationEventHandler;
                        if (handler == null) continue;
                        using dynamic eventData = JObject.Parse(message);
                        await Task.Yield();
                        await handler.Handle(eventData);
                    }
                    else
                    {
                        var handler = scope.ResolveOptional(subscription.HandlerType);
                        if (handler == null) continue;
                        var eventType = _subsManager.GetEventTypeByName(eventName);
                        var integrationEvent = JsonConvert.DeserializeObject(message, eventType);
                        var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);

                        await Task.Yield();
                        await (Task)concreteType.GetMethod("Handle").Invoke(handler, new object[] { integrationEvent });
                    }
                }
            }
        }
        else
        {
            Logger.Warning($"No subscription for RabbitMQ event: {eventName}");
        }
    }

    /// <summary>
    /// 进程事件（使用自带的）
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    private async Task ProcessEventByNetCore(string eventName, string message)
    {
        Logger.Information($"Processing RabbitMQ event: {eventName}");
        if (_subsManager.HasSubscriptionsForEvent(eventName))
        {
            //安装 Microsoft.Extensions.DependencyInjection扩展包

            using (var scope = _serviceProvider.CreateScope())
            {
                var subscriptions = _subsManager.GetHandlersForEvent(eventName);
                foreach (var subscription in subscriptions)
                {
                    if (subscription.IsDynamic)
                    {
                        var handler =
                            scope.ServiceProvider.GetRequiredService(subscription.HandlerType) as
                                IDynamicIntegrationEventHandler;
                        if (handler == null) continue;
                        using dynamic eventData = JObject.Parse(message);
                        await Task.Yield();
                        await handler.Handle(eventData);
                    }
                    else
                    {
                        var handler = scope.ServiceProvider.GetRequiredService(subscription.HandlerType);
                        if (handler == null) continue;
                        var eventType = _subsManager.GetEventTypeByName(eventName);
                        var integrationEvent = JsonConvert.DeserializeObject(message, eventType);
                        var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);

                        await Task.Yield();
                        await (Task)concreteType.GetMethod("Handle").Invoke(handler, new object[] { integrationEvent });
                    }
                }
            }
        }
        else
        {
            Logger.Warning($"No subscription for RabbitMQ event: {eventName}");
        }
    }

    #endregion

    #region 释放

    /// <summary>
    /// 释放
    /// </summary>
    public void Dispose()
    {
        if (_consumerChannel != null)
        {
            _consumerChannel.Dispose();
        }

        _subsManager.Clear();
    }

    #endregion
}