using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Ape.Volo.Common.Caches;
using Ape.Volo.Common.Global;
using Ape.Volo.Common.Internal;
using Ape.Volo.Common.WebApp;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Ape.Volo.Common;

/// <summary>
/// 全局应用类
/// 实现参考 https://github.com/MonkSoul/Furion/blob/v4/framework/Furion/App/App.cs
/// </summary>
public static class App
{
    /// <summary>
    /// 全局配置选项
    /// </summary>
    public static IConfiguration Configuration => InternalApp.Configuration;

    /// <summary>
    /// 获取Web主机环境，如，是否是开发环境，生产环境等
    /// </summary>
    public static IWebHostEnvironment WebHostEnvironment => InternalApp.WebHostEnvironment;

    /// <summary>
    /// 获取泛型主机环境，如，是否是开发环境，生产环境等
    /// </summary>
    public static IHostEnvironment HostEnvironment => InternalApp.HostEnvironment;

    /// <summary>
    /// 存储根服务，可能为空
    /// </summary>
    public static IServiceProvider RootServices => InternalApp.RootServices;


    /// <summary>
    /// 获取请求上下文
    /// </summary>
    public static HttpContext HttpContext =>
        CatchOrDefault(() => RootServices?.GetService<IHttpContextAccessor>()?.HttpContext);

    public static IHttpUser HttpUser => GetService<IHttpUser>();

    public static ICache Cache => GetService<ICache>();

    public static IMapper Mapper => GetService<IMapper>();

    /// <summary>
    /// 未托管的对象集合
    /// </summary>
    public static readonly ConcurrentBag<IDisposable> UnmanagedObjects;


    /// <summary>
    /// 解析服务提供器
    /// </summary>
    /// <param name="serviceType"></param>
    /// <returns></returns>
    public static IServiceProvider GetServiceProvider(Type serviceType)
    {
        // 处理控制台应用程序
        if (HostEnvironment == default) return RootServices;

        // 第一选择，判断是否是单例注册且单例服务不为空，如果是直接返回根服务提供器
        if (RootServices != null && Enumerable.Where<ServiceDescriptor>(InternalApp.InternalServices, u =>
                    u.ServiceType == (serviceType.IsGenericType ? serviceType.GetGenericTypeDefinition() : serviceType))
                .Any(u => u.Lifetime == ServiceLifetime.Singleton))
        {
            return RootServices;
        }

        // 第二选择是获取 HttpContext 对象的 RequestServices
        var httpContext = HttpContext;
        if (httpContext?.RequestServices != null)
        {
            return httpContext.RequestServices;
        }

        // 第三选择，创建新的作用域并返回服务提供器
        if (RootServices != null)
        {
            var scoped = RootServices.CreateScope();
            UnmanagedObjects.Add(scoped);
            return scoped.ServiceProvider;
        }

        // 第四选择，构建新的服务对象（性能最差）
        var serviceProvider =
            ServiceCollectionContainerBuilderExtensions.BuildServiceProvider(InternalApp.InternalServices);
        UnmanagedObjects.Add(serviceProvider);
        return serviceProvider;
    }

    /// <summary>
    /// 获取请求生存周期的服务
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <param name="serviceProvider"></param>
    /// <returns></returns>
    public static TService GetService<TService>(IServiceProvider serviceProvider = default)
        where TService : class
    {
        return GetService(typeof(TService), serviceProvider) as TService;
    }

    /// <summary>
    /// 获取请求生存周期的服务
    /// </summary>
    /// <param name="type"></param>
    /// <param name="serviceProvider"></param>
    /// <returns></returns>
    public static object GetService(Type type, IServiceProvider serviceProvider = default)
    {
        return (serviceProvider ?? GetServiceProvider(type)).GetService(type);
    }

    /// <summary>
    /// 获取请求生存周期的服务集合
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <param name="serviceProvider"></param>
    /// <returns></returns>
    public static IEnumerable<TService> GetServices<TService>(IServiceProvider serviceProvider = default)
        where TService : class
    {
        return (serviceProvider ?? GetServiceProvider(typeof(TService))).GetServices<TService>();
    }

    /// <summary>
    /// 获取请求生存周期的服务集合
    /// </summary>
    /// <param name="type"></param>
    /// <param name="serviceProvider"></param>
    /// <returns></returns>
    public static IEnumerable<object> GetServices(Type type, IServiceProvider serviceProvider = default)
    {
        return (serviceProvider ?? GetServiceProvider(type)).GetServices(type);
    }

    /// <summary>
    /// 获取请求生存周期的服务
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <param name="serviceProvider"></param>
    /// <returns></returns>
    public static TService GetRequiredService<TService>(IServiceProvider serviceProvider = default)
        where TService : class
    {
        return GetRequiredService(typeof(TService), serviceProvider) as TService;
    }

    /// <summary>
    /// 获取请求生存周期的服务
    /// </summary>
    /// <param name="type"></param>
    /// <param name="serviceProvider"></param>
    /// <returns></returns>
    public static object GetRequiredService(Type type, IServiceProvider serviceProvider = default)
    {
        return (serviceProvider ?? GetServiceProvider(type)).GetRequiredService(type);
    }

    #region Options

    /// <summary>获取选项</summary>
    /// <typeparam name="TOptions">强类型选项类</typeparam>
    /// <param name="serviceProvider"></param>
    /// <returns>TOptions</returns>
    public static TOptions GetOptions<TOptions>(IServiceProvider serviceProvider = null) where TOptions : class, new()
    {
        IOptions<TOptions> service = GetService<IOptions<TOptions>>(serviceProvider ?? RootServices);
        return service?.Value;
    }

    /// <summary>获取选项</summary>
    /// <typeparam name="TOptions">强类型选项类</typeparam>
    /// <param name="serviceProvider"></param>
    /// <returns>TOptions</returns>
    public static TOptions GetOptionsMonitor<TOptions>(IServiceProvider serviceProvider = null)
        where TOptions : class, new()
    {
        IOptionsMonitor<TOptions> service =
            GetService<IOptionsMonitor<TOptions>>(serviceProvider ?? RootServices);
        return service?.CurrentValue;
    }

    /// <summary>获取选项</summary>
    /// <typeparam name="TOptions">强类型选项类</typeparam>
    /// <param name="serviceProvider"></param>
    /// <returns>TOptions</returns>
    public static TOptions GetOptionsSnapshot<TOptions>(IServiceProvider serviceProvider = null)
        where TOptions : class, new()
    {
        IOptionsSnapshot<TOptions> service = GetService<IOptionsSnapshot<TOptions>>(serviceProvider);
        return service?.Value;
    }

    #endregion

    /// <summary>
    /// 获取当前线程 Id
    /// </summary>
    /// <returns></returns>
    public static int GetThreadId()
    {
        return Environment.CurrentManagedThreadId;
    }

    /// <summary>
    /// 获取当前请求 TraceId
    /// </summary>
    /// <returns></returns>
    public static string GetTraceId()
    {
        return Activity.Current?.Id ?? (InternalApp.RootServices == null ? default : HttpContext?.TraceIdentifier);
    }

    /// <summary>
    /// 获取一段代码执行耗时
    /// </summary>
    /// <param name="action">委托</param>
    /// <returns><see cref="long"/></returns>
    public static long GetExecutionTime(Action action)
    {
        // 空检查
        if (action == null) throw new ArgumentNullException(nameof(action));

        // 计算接口执行时间
        var timeOperation = Stopwatch.StartNew();
        action();
        timeOperation.Stop();
        return timeOperation.ElapsedMilliseconds;
    }

    /// <summary>
    /// 获取服务注册的生命周期类型
    /// </summary>
    /// <param name="serviceType"></param>
    /// <returns></returns>
    public static ServiceLifetime? GetServiceLifetime(Type serviceType)
    {
        var serviceDescriptor = Enumerable
            .FirstOrDefault<ServiceDescriptor>(InternalApp.InternalServices, u =>
                u.ServiceType == (serviceType.IsGenericType ? serviceType.GetGenericTypeDefinition() : serviceType));

        return serviceDescriptor?.Lifetime;
    }


    /// <summary>
    /// 构造函数
    /// </summary>
    static App()
    {
        // 未托管的对象
        UnmanagedObjects = new ConcurrentBag<IDisposable>();
    }


    /// <summary>
    /// GC 回收默认间隔
    /// </summary>
    private const int GC_COLLECT_INTERVAL_SECONDS = 5;

    /// <summary>
    /// 记录最近 GC 回收时间
    /// </summary>
    private static DateTime? LastGCCollectTime { get; set; }

    /// <summary>
    /// 释放所有未托管的对象
    /// </summary>
    public static void DisposeUnmanagedObjects()
    {
        foreach (var dsp in UnmanagedObjects)
        {
            try
            {
                dsp?.Dispose();
            }
            finally
            {
            }
        }

        // 强制手动回收 GC 内存
        if (UnmanagedObjects.Any())
        {
            var nowTime = DateTime.UtcNow;
            if ((LastGCCollectTime == null ||
                 (nowTime - LastGCCollectTime.Value).TotalSeconds > GC_COLLECT_INTERVAL_SECONDS))
            {
                LastGCCollectTime = nowTime;
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        UnmanagedObjects.Clear();
    }

    /// <summary>
    /// 处理获取对象异常问题
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <param name="action">获取对象委托</param>
    /// <param name="defaultValue">默认值</param>
    /// <returns>T</returns>
    private static T CatchOrDefault<T>(Func<T> action, T defaultValue = null)
        where T : class
    {
        try
        {
            return action();
        }
        catch
        {
            return defaultValue ?? null;
        }
    }
}
