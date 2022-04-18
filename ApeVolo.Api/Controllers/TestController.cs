using System.Threading.Tasks;
using ApeVolo.Api.ActionExtension.Sign;
using ApeVolo.Api.Controllers.Base;
using ApeVolo.Api.MQ.Rabbit.Events;
using ApeVolo.Common.Caches.Redis.Service;
using ApeVolo.Common.Caches.Redis.Service.MessageQueue;
using ApeVolo.Entity.Do.Core;
using ApeVolo.EventBus.Abstractions;
using ApeVolo.IBusiness.Interface.Core;
using ApeVolo.IBusiness.Interface.Email;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApeVolo.Api.Controllers;

/// <summary>
/// 测试
/// </summary>
[Route("api/[controller]/[action]")]
//[ApiController]
//[Authorize(Policy = GlobalSwitch.AUTH_POLICYS_NAME)]
public class TestController : BaseApiController
{
    private readonly IEmailScheduleTask _emailScheduleTask;
    private readonly IEventBus _eventBus;
    private readonly IRedisCacheService _redisCacheService;
    private readonly ITestApeVoloService _testApeVoloService;

    public TestController(IEmailScheduleTask emailScheduleTask, ITestApeVoloService testApeVoloService,
        IRedisCacheService redisCacheService, IEventBus eventBus)
    {
        _emailScheduleTask = emailScheduleTask;
        _testApeVoloService = testApeVoloService;
        _redisCacheService = redisCacheService;
        _eventBus = eventBus;
    }

    /// <summary>
    /// 邮件测试
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<object>> TestSecret()
    {
        await _emailScheduleTask.ExecuteAsync();
        return Success();
    }

    /// <summary>
    /// Apache JMeter 性能测试
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<object>> AddApeVolo()
    {
        try
        {
            await _testApeVoloService.CreateAsync(new TestApeVolo
            {
                Label = "test",
                Content = "test",
                Sort = 1
            });
        }
        catch
        {
            // ignored
        }

        return Success();
    }

    /// <summary>
    /// 签名测试
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [AllowAnonymous]
    [VerifySignature]
    public async Task<ActionResult<object>> TestSign()
    {
        try
        {
            await Task.CompletedTask;
        }
        catch
        {
            // ignored
        }

        return Success();
    }

    /// <summary>
    /// redismq测试
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<object>> TestRedisMq()
    {
        try
        {
            await _redisCacheService.ListLeftPushAsync(MqTopicNameKey.MailboxQueue, "123456789");
        }
        catch
        {
            // ignored
        }

        return Success();
    }


    /// <summary>
    /// rabbitmq测试
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [AllowAnonymous]
    public ActionResult<object> EventBusTry()
    {
        try
        {
            var userId = 1306054134645919750;
            for (var i = 1; i <= 1000; i++)
            {
                var eventMessage = new UserQueryIntegrationEvent(userId, i);
                _eventBus.Publish(eventMessage);
            }
        }
        catch
        {
            // ignored
        }

        return Success();
    }
}