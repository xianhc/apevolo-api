using System.Threading.Tasks;
using ApeVolo.Api.ActionExtension.Sign;
using ApeVolo.Api.Controllers.Base;
using ApeVolo.Common.Caches.Redis.Service;
using ApeVolo.Common.Caches.Redis.Service.MessageQueue;
using ApeVolo.Common.Resources;
using ApeVolo.Entity.Permission.Test;
using ApeVolo.IBusiness.Interface.Message.Email;
using ApeVolo.IBusiness.Interface.Permission.Role;
using ApeVolo.IBusiness.Interface.Permission.User;
using ApeVolo.IBusiness.Interface.Test;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shyjus.BrowserDetection;

namespace ApeVolo.Api.Controllers.Test;

/// <summary>
/// 测试
/// </summary>
[Route("api/[controller]/[action]")]
//[ApiController]
//[Authorize(Policy = GlobalSwitch.AUTH_POLICYS_NAME)]
public class TestController : BaseApiController
{
    private readonly IEmailScheduleTask _emailScheduleTask;

    //private readonly IEventBus _eventBus;
    private readonly IRedisCacheService _redisCacheService;
    private readonly ITestApeVoloService _testApeVoloService;
    private readonly IUserService _userService;
    private readonly IRoleService _roleService;
    private readonly IBrowserDetector _browserDetector;

    public TestController(IEmailScheduleTask emailScheduleTask, ITestApeVoloService testApeVoloService,
        IRedisCacheService redisCacheService, IUserService userService, IRoleService roleService,
        IBrowserDetector browserDetector)
    {
        _emailScheduleTask = emailScheduleTask;
        _testApeVoloService = testApeVoloService;
        _redisCacheService = redisCacheService;
        //_eventBus = eventBus;
        _userService = userService;
        _roleService = roleService;
        _browserDetector = browserDetector;
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


    /*[HttpGet]
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
    }*/

    /// <summary>
    /// redismq测试
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<object>> Test123456()
    {
        string text = "";
        try
        {
            text = Localized.Get("Job");
        }
        catch
        {
            // ignored
        }

        await Task.CompletedTask;
        return Success();
    }
}