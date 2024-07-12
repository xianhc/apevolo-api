using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Ape.Volo.Common.Attributes.Redis;
using Ape.Volo.Common.Caches.Redis.Models;
using Ape.Volo.Common.Extensions;
using Ape.Volo.Common.Helper;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using StackExchange.Redis;

namespace Ape.Volo.Common.Caches.Redis.MessageQueue;

public class InitCore
{
    /// <summary>
    /// 立即消费
    /// </summary>
    /// <param name="executorDescriptorList"></param>
    /// <param name="queueOptions"></param>
    private async Task Send(IEnumerable<ConsumerExecutorDescriptor> executorDescriptorList,
        RedisQueueOptions queueOptions)
    {
        var tasks = new List<Task>();

        foreach (var consumerExecutorDescriptor in executorDescriptorList)
        {
            tasks.Add(ProcessConsumer(consumerExecutorDescriptor, queueOptions));
        }

        await Task.WhenAll(tasks);
    }

    private async Task ProcessConsumer(ConsumerExecutorDescriptor consumerExecutorDescriptor,
        RedisQueueOptions queueOptions)
    {
        using (var scope = App.GetRequiredService<IServiceScopeFactory>().CreateScope())
        {
            var publish = consumerExecutorDescriptor.Attribute.Name;
            var bulk = consumerExecutorDescriptor.Attribute.Bulk;
            var obj = ActivatorUtilities.GetServiceOrCreateInstance(scope.ServiceProvider,
                consumerExecutorDescriptor.ImplTypeInfo);
            var redis = scope.ServiceProvider.GetService<ICache>();
            var parameterInfos = consumerExecutorDescriptor.MethodInfo.GetParameters();

            while (true)
            {
                try
                {
                    if (queueOptions.ShowLog)
                    {
                        Log.Information($"执行方法:{obj},key:{publish},执行时间{DateTime.Now}");
                    }

                    var count = await redis.GetDatabase().ListLengthAsync(publish);
                    if (count > 0)
                    {
                        if (bulk)
                        {
                            List<RedisValue> redisValues = new List<RedisValue>();

                            for (int i = 0; i < queueOptions.MaxQueueConsumption; i++)
                            {
                                var res = await redis.GetDatabase().ListRightPopAsync(publish);
                                if (res.HasValue)
                                {
                                    redisValues.Add(res);
                                }
                                else
                                {
                                    break;
                                }
                            }

                            object[] parameters = { redisValues };
                            consumerExecutorDescriptor.MethodInfo.Invoke(obj, parameters);
                            if (queueOptions.IntervalTime > 0)
                            {
                                await Task.Delay(queueOptions.IntervalTime);
                            }
                        }
                        else
                        {
                            var res = await redis.GetDatabase().ListRightPopAsync(publish);
                            if (string.IsNullOrEmpty(res)) continue;
                            if (parameterInfos.Length == 0)
                            {
                                consumerExecutorDescriptor.MethodInfo.Invoke(obj, null);
                            }
                            else
                            {
                                object[] parameters = { res };
                                consumerExecutorDescriptor.MethodInfo.Invoke(obj, parameters);
                            }

                            if (queueOptions.IntervalTime > 0)
                            {
                                await Task.Delay(queueOptions.IntervalTime);
                            }
                        }
                    }
                    else
                    {
                        await Task.Delay(queueOptions.SuspendTime);
                    }
                }
                catch (System.Exception ex)
                {
                    Log.Fatal($"消费队列报错key:{publish}\n\r" + ExceptionHelper.GetExceptionAllMsg(ex));
                }
            }
        }
    }

    /// <summary>
    /// 延迟消费
    /// </summary>
    /// <param name="executorDescriptorList"></param>
    /// <param name="queueOptions"></param>
    private async Task SendDelay(IEnumerable<ConsumerExecutorDescriptor> executorDescriptorList,
        RedisQueueOptions queueOptions)
    {
        List<Task> tasks = new List<Task>();
        foreach (var consumerExecutorDescriptor in executorDescriptorList)
        {
            //线程
            tasks.Add(Task.Run(() =>
            {
                using (var scope = App.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    var publish = $"queue:{consumerExecutorDescriptor.Attribute.Name}";
                    var bulk = consumerExecutorDescriptor.Attribute.Bulk;
                    var provider = scope.ServiceProvider;
                    var obj = ActivatorUtilities.GetServiceOrCreateInstance(provider,
                        consumerExecutorDescriptor.ImplTypeInfo);
                    ParameterInfo[] parameterInfos = consumerExecutorDescriptor.MethodInfo.GetParameters();
                    //redis对象
                    var redis = scope.ServiceProvider.GetService<ICache>();

                    //从zset添加到队列(锁)
                    tasks.Add(Task.Run(async () =>
                    {
                        while (true)
                        {
                            var keyInfo = "lockZSetTibos"; //锁名称
                            var token = Guid.NewGuid().ToString("N"); //锁持有者
                            var coon = await redis.GetDatabase().LockTakeAsync(keyInfo, token,
                                TimeSpan.FromSeconds(5));
                            if (coon)
                            {
                                try
                                {
                                    var stopTimeStamp = DateTime.Now.ToUnixTimeStampSecond();
                                    var arry = await redis.GetDatabase().SortedSetRangeByScoreAsync(
                                        consumerExecutorDescriptor.Attribute.Name, double.NegativeInfinity,
                                        stopTimeStamp);
                                    if (arry is { Length: > 0 })
                                    {
                                        foreach (var item in arry)
                                        {
                                            await redis.GetDatabase().ListLeftPushAsync(publish, item);
                                        }

                                        //移除zset数据
                                        await redis.GetDatabase().SortedSetRemoveRangeByScoreAsync(
                                            consumerExecutorDescriptor.Attribute.Name, double.NegativeInfinity,
                                            stopTimeStamp);
                                    }
                                    else
                                    {
                                        await Task.Delay(queueOptions.SuspendTime);
                                    }
                                }
                                catch (System.Exception ex)
                                {
                                    Log.Fatal($"延迟消费队列报错key:{publish}\n\r" + ExceptionHelper.GetExceptionAllMsg(ex));
                                }
                                finally
                                {
                                    //释放锁
                                    await redis.GetDatabase().LockReleaseAsync(keyInfo, token);
                                }
                            }
                        }
                    }));
                    //消费队列
                    tasks.Add(Task.Run(async () =>
                    {
                        while (true)
                        {
                            try
                            {
                                if (queueOptions.ShowLog)
                                {
                                    Log.Information($"执行方法:{obj},key:{publish},执行时间{DateTime.Now}");
                                }

                                var count = await redis.GetDatabase().ListLengthAsync(publish);
                                if (count > 0)
                                {
                                    if (bulk)
                                    {
                                        List<RedisValue> redisValues = new List<RedisValue>();

                                        for (int i = 0; i < queueOptions.MaxQueueConsumption; i++)
                                        {
                                            var res = await redis.GetDatabase().ListRightPopAsync(publish);
                                            if (res.HasValue)
                                            {
                                                redisValues.Add(res);
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }

                                        object[] parameters = { redisValues };
                                        consumerExecutorDescriptor.MethodInfo.Invoke(obj, parameters);
                                        if (queueOptions.IntervalTime > 0)
                                        {
                                            await Task.Delay(queueOptions.IntervalTime);
                                        }
                                    }
                                    else
                                    {
                                        var res = await redis.GetDatabase().ListRightPopAsync(publish);
                                        if (string.IsNullOrEmpty(res)) continue;
                                        if (parameterInfos.Length == 0)
                                        {
                                            consumerExecutorDescriptor.MethodInfo.Invoke(obj, null);
                                        }
                                        else
                                        {
                                            object[] parameters = { res };
                                            consumerExecutorDescriptor.MethodInfo.Invoke(obj, parameters);
                                        }

                                        if (queueOptions.IntervalTime > 0)
                                        {
                                            await Task.Delay(queueOptions.IntervalTime);
                                        }
                                    }
                                }
                                else
                                {
                                    await Task.Delay(queueOptions.SuspendTime);
                                }
                            }
                            catch (System.Exception ex)
                            {
                                Log.Fatal($"消费队列报错key:{publish}\n\r" + ExceptionHelper.GetExceptionAllMsg(ex));
                            }
                        }
                    }));
                }
            }));
        }

        await Task.WhenAll(tasks);
    }

    public async Task FindInterfaceTypes(RedisQueueOptions queueOptions)
    {
        var executorDescriptorList = new List<ConsumerExecutorDescriptor>();
        await using var scoped = App.RootServices.CreateAsyncScope();
        var scopedProvider = scoped.ServiceProvider;
        var listService = scopedProvider.GetService<Func<Type, IRedisSubscribe>>();
        foreach (var item in queueOptions.ListSubscribe)
        {
            if (listService != null)
            {
                var consumerServices = listService(item);
                var typeInfo = consumerServices.GetType().GetTypeInfo();
                if (!typeof(IRedisSubscribe).GetTypeInfo().IsAssignableFrom(typeInfo))
                {
                    continue;
                }

                executorDescriptorList.AddRange(GetTopicAttributesDescription(typeInfo));
            }
        }

        List<Task> tasks = new List<Task>();
        //普通队列任务
        tasks.Add(Send(executorDescriptorList.Where(m => m.Attribute.GetType().Name == nameof(SubscribeAttribute)),
            queueOptions));

        //延迟队列任务
        tasks.Add(SendDelay(
            executorDescriptorList.Where(m => m.Attribute.GetType().Name == nameof(SubscribeDelayAttribute)),
            queueOptions));
        await Task.WhenAll(tasks);
    }


    private IEnumerable<ConsumerExecutorDescriptor> GetTopicAttributesDescription(TypeInfo typeInfo)
    {
        foreach (var method in typeInfo.DeclaredMethods)
        {
            var topicAttr = method.GetCustomAttributes<TopicAttribute>(true);
            var topicAttributes = topicAttr as IList<TopicAttribute> ?? topicAttr.ToList();

            if (!topicAttributes.Any())
            {
                continue;
            }

            foreach (var attr in topicAttributes)
            {
                yield return InitDescriptor(attr, method, typeInfo);
            }
        }
    }


    private ConsumerExecutorDescriptor InitDescriptor(TopicAttribute attr, MethodInfo methodInfo, TypeInfo implType)
    {
        var descriptor = new ConsumerExecutorDescriptor
        {
            Attribute = attr,
            MethodInfo = methodInfo,
            ImplTypeInfo = implType
        };

        return descriptor;
    }
}
