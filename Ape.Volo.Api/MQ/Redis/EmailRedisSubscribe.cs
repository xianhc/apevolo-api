using System.Threading.Tasks;
using Ape.Volo.Common.Attributes.Redis;
using Ape.Volo.Common.Caches.Redis.MessageQueue;
using Ape.Volo.IBusiness.Interface.Message.Email;
using Microsoft.Extensions.Logging;

namespace Ape.Volo.Api.MQ.Redis;

public class EmailRedisSubscribe : IRedisSubscribe
{
    #region Fields

    private readonly ILogger<EmailRedisSubscribe> _logger;
    private readonly IEmailScheduleTask _emailScheduleTask;

    #endregion

    #region Ctor

    public EmailRedisSubscribe(IEmailScheduleTask emailScheduleTask, ILogger<EmailRedisSubscribe> logger)
    {
        _emailScheduleTask = emailScheduleTask;
        _logger = logger;
    }

    #endregion

    [Subscribe(MqTopicNameKey.MailboxQueue)]
    private async Task DoSub(long emailId)
    {
        _logger.LogInformation($"消费ID：{emailId}");
        await _emailScheduleTask.ExecuteAsync(emailId);
        //发送失败是否需要重回队列？？？
        //   await Task.CompletedTask;
    }
}
