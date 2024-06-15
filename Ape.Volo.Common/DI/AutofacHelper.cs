using System;
using Ape.Volo.Common.Helper.Serilog;
using Autofac;
using Autofac.Core.Registration;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace Ape.Volo.Common.DI;

/// <summary>
/// AutoFac帮助
/// </summary>
public static class AutofacHelper
{
    private static readonly ILogger Logger = SerilogManager.GetLogger(typeof(AutofacHelper));

    public static ILifetimeScope Container { get; set; }
    // private static readonly Lazy<ILifetimeScope> LazyContainer = new(() => new ContainerBuilder().Build());
    //
    // public static ILifetimeScope Container => LazyContainer.Value;

    /// <summary>
    /// 优先从当前 HTTP 请求范围内获取服务，如果不可用则从全局容器获取服务
    /// </summary>
    /// <typeparam name="T">接口类型</typeparam>
    /// <returns>返回服务实例，如果服务未注册则返回 null</returns>
    public static T GetService<T>() where T : class
    {
        var httpContextAccessor = Container.Resolve<IHttpContextAccessor>();

        var requestServices = httpContextAccessor?.HttpContext?.RequestServices;
        var service = requestServices?.GetService(typeof(T));
        if (service is T typedService)
        {
            return typedService;
        }

        try
        {
            return Container.Resolve<T>();
        }
        catch (ComponentNotRegisteredException)
        {
            Logger.Fatal($" 类型服务{typeof(T)} 未注册。");
            return null;
        }
        catch (System.Exception ex)
        {
            Logger.Fatal($"无法解析类型为 {typeof(T)} 的服务。{ex.Message}");
            throw new InvalidOperationException($"无法解析类型为 {typeof(T)} 的服务。", ex);
        }
    }
}
