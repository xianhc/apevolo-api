using System;
using Ape.Volo.Common.Extensions;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Profiling;

namespace Ape.Volo.Api.Extensions;

/// <summary>
/// MiniProfiler性能监控启动器
/// </summary>
public static class MiniProfilerSetup
{
    public static void AddMiniProfilerSetup(this IServiceCollection services)
    {
        if (services.IsNull()) throw new ArgumentNullException(nameof(services));

        services.AddMiniProfiler(options =>
            {
                options.RouteBasePath = "/profiler";
                //(options.Storage as MemoryCacheStorage).CacheDuration = TimeSpan.FromMinutes(10);
                options.PopupRenderPosition = RenderPosition.Left;
                options.PopupShowTimeWithChildren = true;

                // 可以增加权限
                //options.ResultsAuthorize = request => request.HttpContext.User.IsInRole("Admin");
                //options.UserIdProvider = request => request.HttpContext.User.Identity.Name;
            }
        ).AddEntityFramework();
    }
}
