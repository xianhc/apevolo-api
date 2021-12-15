using System.Threading.Tasks;
using ApeVolo.Common.Caches.Redis.Attributes;
using ApeVolo.Common.Caches.Redis.Models;
using ApeVolo.Common.Caches.Redis.Service.MessageQueue;
using ApeVolo.Common.Helper;
using ApeVolo.IBusiness.Interface.Email;

namespace ApeVolo.Api.MQ.Redis
{
    public class EmailRedisSubscribe : IRedisSubscribe
    {
        #region Fields

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
            LogHelper.WriteLog(text, new[] { "EmailRedisSubscribe订阅" });
            await _emailScheduleTask.ExecuteAsync(emailId);
            //发送失败是否需要重回队列？？？
            //   await Task.CompletedTask;
        }
    }
}