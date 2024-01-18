using System;
using System.Threading.Tasks;
using Ape.Volo.IBusiness.Interface.System;
using Ape.Volo.QuartzNetService.service;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Ape.Volo.QuartzNetService;

/// <summary>
/// 测试控制台打印作业
/// </summary>
public class TestConsoleWriteJobService : JobBase, IJob
{
    public TestConsoleWriteJobService(ISchedulerCenterService schedulerCenterService,
        IQuartzNetService quartzNetService, IQuartzNetLogService quartzNetLogService,
        ILogger<TestConsoleWriteJobService> logger)
    {
        QuartzNetService = quartzNetService;
        QuartzNetLogService = quartzNetLogService;
        SchedulerCenterService = schedulerCenterService;
        Logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        await ExecuteJob(context, async () => await Run(context));
    }

    private async Task Run(IJobExecutionContext context)
    {
        await Console.Out.WriteLineAsync("当前时间：" + DateTime.Now + "\n");
        //获取传递参数
        JobDataMap data = context.JobDetail.JobDataMap;
    }
}
