using System;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Spi;

namespace Ape.Volo.QuartzNetService.service;

/// <summary>
/// 任务工厂
/// </summary>
public class JobFactory : IJobFactory
{
    #region 字段

    private readonly IServiceProvider _serviceProvider;

    #endregion

    #region 构造函数

    public JobFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    #endregion

    #region 方法

    /// <summary>
    /// 实现接口Job
    /// </summary>
    /// <param name="bundle"></param>
    /// <param name="scheduler"></param>
    /// <returns></returns>
    public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
    {
        var serviceScope = _serviceProvider.CreateScope();
        var job = serviceScope.ServiceProvider.GetService(bundle.JobDetail.JobType) as IJob;
        return job;
    }

    public void ReturnJob(IJob job)
    {
        var disposable = job as IDisposable;
        if (disposable != null)
        {
            disposable.Dispose();
        }
    }

    #endregion
}
