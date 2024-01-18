using System.Threading.Tasks;
using Ape.Volo.Common.Caches.Redis.Attributes;
using Ape.Volo.Common.Caches.Redis.MessageQueue;
using Ape.Volo.Common.Caches.Redis.Models;
using Ape.Volo.Common.Helper.Serilog;
using Ape.Volo.IBusiness.Interface.Message.Email;
using Serilog;

namespace Ape.Volo.Api.MQ.Redis;

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
