using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Ape.Volo.Common.Caches.Redis.Abstractions;
using Ape.Volo.Common.Caches.Redis.Attributes;
using Ape.Volo.Common.Caches.Redis.Models;
using Ape.Volo.Common.Extention;
using Microsoft.Extensions.DependencyInjection;

namespace Ape.Volo.Common.Caches.Redis.MessageQueue;

public class InitCore
{
    private async Task Send(IEnumerable<ConsumerExecutorDescriptor> executorDescriptorList,
        IServiceProvider serviceProvider, RedisQueueOptions queueOptions)
    {
        List<Task> tasks = new List<Task>();
        foreach (var consumerExecutorDescriptor in executorDescriptorList)
        {
            //线程
            tasks.Add(Task.Run(async () =>
            {
                using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    var publish = consumerExecutorDescriptor.Attribute.Name;
                    var provider = scope.ServiceProvider;
                    var obj = ActivatorUtilities.GetServiceOrCreateInstance(provider,
                        consumerExecutorDescriptor.ImplTypeInfo);
                    ParameterInfo[] parameterInfos = consumerExecutorDescriptor.MethodInfo.GetParameters();
                    //redis对象
                    var redis = scope.ServiceProvider.GetService<ICache>();
                    while (true)
                    {
                        try
                        {
                            if (queueOptions.ShowLog)
                            {
                                Console.WriteLine($"执行方法:{obj},key:{publish},执行时间{DateTime.Now}");
                            }

                            var count = await redis.GetDatabase().ListLengthAsync(publish);
                            if (count > 0)
                            {
                                //从MQ里获取一条消息
                                var res = await redis.GetDatabase().ListRightPopAsync(publish);
                                if (string.IsNullOrEmpty(res)) continue;
                                //堵塞
                                await Task.Delay(queueOptions.IntervalTime);
                                try
                                {
                                    await Task.Run(async () =>
                                    {
                                        if (parameterInfos.Length == 0)
                                        {
                                            consumerExecutorDescriptor.MethodInfo.Invoke(obj, null);
                                        }
                                        else
                                        {
                                            object[] parameters = { res };
                                            consumerExecutorDescriptor.MethodInfo.Invoke(obj, parameters);
                                        }
                                    });
                                }
                                catch (System.Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                            }
                            else
                            {
                                //线程挂起1s
                                await Task.Delay(queueOptions.SuspendTime);
                            }
                        }
                        catch (System.Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                }
            }));
        }

        await Task.WhenAll(tasks);
    }


    private async Task SendDelay(IEnumerable<ConsumerExecutorDescriptor> executorDescriptorList,
        IServiceProvider serviceProvider, RedisQueueOptions queueOptions)
    {
        List<Task> tasks = new List<Task>();
        foreach (var consumerExecutorDescriptor in executorDescriptorList)
        {
            //线程
            tasks.Add(Task.Run(async () =>
            {
                using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    var publish = $"queue:{consumerExecutorDescriptor.Attribute.Name}";
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
                                    if (arry != null && arry.Length > 0)
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
                                        //线程挂起1s
                                        await Task.Delay(1000);
                                    }
                                }
                                catch (System.Exception ex)
                                {
                                    Console.WriteLine($"执行延迟队列报错:{ex.Message}");
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
                                    Console.WriteLine($"执行方法:{obj},key:{publish},执行时间{DateTime.Now}");
                                }

                                var count = await redis.GetDatabase().ListLengthAsync(publish);
                                if (count > 0)
                                {
                                    //从MQ里获取一条消息
                                    var res = await redis.GetDatabase().ListRightPopAsync(publish);
                                    if (string.IsNullOrEmpty(res)) continue;
                                    //堵塞
                                    await Task.Delay(queueOptions.IntervalTime);
                                    try
                                    {
                                        await Task.Run(async () =>
                                        {
                                            if (parameterInfos.Length == 0)
                                            {
                                                consumerExecutorDescriptor.MethodInfo.Invoke(obj, null);
                                            }
                                            else
                                            {
                                                object[] parameters = { res };
                                                consumerExecutorDescriptor.MethodInfo.Invoke(obj, parameters);
                                            }
                                        });
                                    }
                                    catch (System.Exception ex)
                                    {
                                        Console.WriteLine(ex.Message);
                                    }
                                }
                                else
                                {
                                    //线程挂起1s
                                    await Task.Delay(queueOptions.SuspendTime);
                                }
                            }
                            catch (System.Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                        }
                    }));
                }
            }));
        }

        await Task.WhenAll(tasks);
    }

    public async Task FindInterfaceTypes(IServiceProvider provider, RedisQueueOptions queueOptions)
    {
        var executorDescriptorList = new List<ConsumerExecutorDescriptor>();
        using (var scoped = provider.CreateScope())
        {
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
            tasks.Add(Send(executorDescriptorList.Where(m => m.Attribute.GetType().Name == "SubscribeAttribute"),
                provider, queueOptions));

            //延迟队列任务
            tasks.Add(SendDelay(
                executorDescriptorList.Where(m => m.Attribute.GetType().Name == "SubscribeDelayAttribute"),
                provider, queueOptions));
            await Task.WhenAll(tasks);
        }
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
