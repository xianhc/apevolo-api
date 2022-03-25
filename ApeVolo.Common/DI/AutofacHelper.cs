using Autofac;
using Microsoft.AspNetCore.Http;

namespace ApeVolo.Common.DI;

/// <summary>
/// AutoFac帮助
/// </summary>
public static class AutofacHelper
{
    public static ILifetimeScope Container { get; set; }

    /// <summary>
    /// 获取全局服务
    /// </summary>
    /// <typeparam name="T">接口类型</typeparam>
    /// <returns></returns>
    public static T GetService<T>() where T : class
    {
        return Container.Resolve<T>();
    }

    /// <summary>
    /// 获取当前请求为生命周期的服务
    /// </summary>
    /// <typeparam name="T">接口类型</typeparam>
    /// <returns></returns>
    public static T GetScopeService<T>() where T : class
    {
        return (T)GetService<IHttpContextAccessor>().HttpContext?.RequestServices.GetService(typeof(T));
    }
}