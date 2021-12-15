using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ApeVolo.Common.Caches.Redis.Abstractions;
using ApeVolo.Common.Caches.Redis.Attributes;
using ApeVolo.Common.Caches.Redis.Models;
using Microsoft.Extensions.DependencyInjection;

namespace ApeVolo.Common.Caches.Redis.Service.MessageQueue
{
    public class InitCore
    {
        private async Task Send(IEnumerable<ConsumerExecutorDescriptor> executorDescriptorList,
            IServiceProvider serviceProvider, RedisOptions options)
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
                        var redis = scope.ServiceProvider.GetService<IRedisCacheService>();
                        while (true)
                        {
                            try
                            {
                                if (options.ShowLog)
                                {
                                    Console.WriteLine($"执行方法:{obj},key:{publish},执行时间{DateTime.Now}");
                                }

                                var count = await redis.ListLengthAsync(publish);
                                if (count > 0)
                                {
                                    //从MQ里获取一条消息
                                    var res = await redis.ListRightPopAsync(publish);
                                    if (string.IsNullOrEmpty(res)) continue;
                                    //堵塞
                                    await Task.Delay(options.IntervalTime);
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
                                                object[] parameters = {res};
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
                                    await Task.Delay(options.SuspendTime);
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
            IServiceProvider serviceProvider, RedisOptions options)
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
                        var redis = scope.ServiceProvider.GetService<IRedisCacheService>();

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
                                        var dt = DateTime.Now;
                                        var arry = await redis.SortedSetRangeByScoreAsync(
                                            consumerExecutorDescriptor.Attribute.Name, null, dt);
                                        if (arry != null && arry.Length > 0)
                                        {
                                            foreach (var item in arry)
                                            {
                                                await redis.ListLeftPushAsync(publish, item);
                                            }

                                            //移除zset数据
                                            await redis.SortedSetRemoveRangeByScoreAsync(
                                                consumerExecutorDescriptor.Attribute.Name, null, dt);
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
                                        redis.GetDatabase().LockRelease(keyInfo, token);
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
                                    if (options.ShowLog)
                                    {
                                        Console.WriteLine($"执行方法:{obj},key:{publish},执行时间{DateTime.Now}");
                                    }

                                    var count = await redis.ListLengthAsync(publish);
                                    if (count > 0)
                                    {
                                        //从MQ里获取一条消息
                                        var res = await redis.ListRightPopAsync(publish);
                                        if (string.IsNullOrEmpty(res)) continue;
                                        //堵塞
                                        await Task.Delay(options.IntervalTime);
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
                                                    object[] parameters = {res};
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
                                        await Task.Delay(options.SuspendTime);
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

        public async Task FindInterfaceTypes(IServiceProvider provider, RedisOptions options)
        {
            var executorDescriptorList = new List<ConsumerExecutorDescriptor>();
            using (var scoped = provider.CreateScope())
            {
                var scopedProvider = scoped.ServiceProvider;
                var listService = scopedProvider.GetService<Func<Type, IRedisSubscribe>>();
                foreach (var item in options.ListSubscribe)
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
                    provider, options));

                //延迟队列任务
                tasks.Add(SendDelay(
                    executorDescriptorList.Where(m => m.Attribute.GetType().Name == "SubscribeDelayAttribute"),
                    provider, options));
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
}