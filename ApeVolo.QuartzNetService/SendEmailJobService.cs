using System.Threading.Tasks;
using ApeVolo.IBusiness.Interface.Message.Email;
using ApeVolo.IBusiness.Interface.System.Task;
using ApeVolo.QuartzNetService.service;
using Microsoft.Extensions.Logging;
using Quartz;

namespace ApeVolo.QuartzNetService;

public class SendEmailJobService : JobBase, IJob
{
    private readonly IEmailScheduleTask _emailScheduleTask;

    public SendEmailJobService(ISchedulerCenterService schedulerCenterService, IQuartzNetService quartzNetService,
        IQuartzNetLogService quartzNetLogService, IEmailScheduleTask emailScheduleTask,
        ILogger<SendEmailJobService> logger)
    {
        QuartzNetService = quartzNetService;
        QuartzNetLogService = quartzNetLogService;
        _emailScheduleTask = emailScheduleTask;
        SchedulerCenterService = schedulerCenterService;
        Logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        await ExecuteJob(context, async () => await Run(context));
    }

    private async Task Run(IJobExecutionContext context)
    {
        await _emailScheduleTask.ExecuteAsync();
        //获取传递参数
        //JobDataMap data = context.JobDetail.JobDataMap;
    }
}