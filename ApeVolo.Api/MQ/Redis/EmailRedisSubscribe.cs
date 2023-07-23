using System.Threading.Tasks;
using ApeVolo.Common.Caches.Redis.Attributes;
using ApeVolo.Common.Caches.Redis.MessageQueue;
using ApeVolo.Common.Caches.Redis.Models;
using ApeVolo.Common.Helper.Serilog;
using ApeVolo.IBusiness.Interface.Message.Email;
using Serilog;

namespace ApeVolo.Api.MQ.Redis;

public class EmailRedisSubscribe : IRedisSubscribe
{
    #region Fields

    private static readonly ILogger Logger = SerilogManager.GetLogger(typeof(EmailRedisSubscribe));
    private readonly IEmailScheduleTask _emailScheduleTask;

    #endregion

    #region Ctor

    public EmailRedisSubscribe(IEmailScheduleTask emailScheduleTask)
    {
        _emailScheduleTask = emailScheduleTask;
    }

    #endregion

    [Subscribe(MqTopicNameKey.MailboxQueue)]
    private async Task DoSub(long emailId)
    {
        var text = $"EmailRedisSubscribe订阅者==》从Redis消息队列:{RedisChannels.ChangeMailbox}==》得到消费信息:{emailId}";
        Logger.Information(text);
        await _emailScheduleTask.ExecuteAsync(emailId);
        //发送失败是否需要重回队列？？？
        //   await Task.CompletedTask;
    }
}
