using System;
using System.IO;
using System.Linq;
using System.Reflection;
using ApeVolo.QuartzNetService.service;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Spi;

namespace ApeVolo.Api.Extensions;

/// <summary>
/// QuartzNet作业启动器
/// </summary>
public static class QuartzNetJobSetup
{
    public static void AddQuartzNetJobSetup(this IServiceCollection services)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));


        services.AddSingleton<IJobFactory, JobFactory>();
        services.AddSingleton<ISchedulerCenterService, SchedulerCenterService>();
        //任务注入
        var baseType = typeof(IJob);
        var path = AppDomain.CurrentDomain.RelativeSearchPath ?? AppDomain.CurrentDomain.BaseDirectory;
        var referencedAssemblies =
            Directory.GetFiles(path, "ApeVolo.QuartzNetService.dll").Select(Assembly.LoadFrom).ToArray();
        var types = referencedAssemblies
            .SelectMany(a => a.DefinedTypes)
            .Select(type => type.AsType())
            .Where(x => x != baseType && baseType.IsAssignableFrom(x)).ToArray();
        var implementTypes = types.Where(x => x.IsClass).ToArray();
        foreach (var implementType in implementTypes)
        {
            services.AddTransient(implementType);
        }
    }
}