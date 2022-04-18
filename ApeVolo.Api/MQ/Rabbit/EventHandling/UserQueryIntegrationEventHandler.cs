using System.Threading.Tasks;
using ApeVolo.Api.MQ.Rabbit.Events;
using ApeVolo.Common.Helper;
using ApeVolo.EventBus.Abstractions;
using ApeVolo.IBusiness.Interface.Core;
using log4net;

namespace ApeVolo.Api.MQ.Rabbit.EventHandling
{
    /// <summary>
    /// 测试rabbitmq事件总线
    /// </summary>
    public class UserQueryIntegrationEventHandler : IIntegrationEventHandler<UserQueryIntegrationEvent>
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(UserQueryIntegrationEventHandler));
        private readonly IUserService _userService;

        public UserQueryIntegrationEventHandler(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// 处理事件
        /// </summary>
        /// <param name="event"></param>
        public async Task Handle(UserQueryIntegrationEvent @event)
        {
            Log.Info($"----- Handling integration event: {@event.Id} at {@event}");
            ConsoleHelper.WriteLine($"----- Handling integration event: {@event.Id} at ApeVolo - ({@event})");
            await _userService.QueryByIdAsync(@event.UserId);
        }
    }
}