using System.Threading.Tasks;
using ApeVolo.Api.ActionExtension.Sign;
using ApeVolo.Api.Controllers.Base;
using ApeVolo.IBusiness.Interface.Test;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApeVolo.Api.Controllers.Test;

/// <summary>
/// 测试
/// </summary>
[Area("Test Manager")]
[Route("api/[controller]/[action]")]
//[ApiController]
public class TestController : BaseApiController
{
    // private readonly IEmailScheduleTask _emailScheduleTask;
    //
    // //private readonly IEventBus _eventBus;
    // private readonly IRedisCacheService _redisCacheService;
    readonly ITestApeVoloService _testApeVoloService;

    // private readonly IUserService _userService;
    // private readonly IRoleService _roleService;
    // private readonly IBrowserDetector _browserDetector;

    public TestController(ITestApeVoloService testApeVoloService)
    {
        _testApeVoloService = testApeVoloService;
        //_eventBus = eventBus;
    }


    // [HttpGet]
    // [AllowAnonymous]
    // public async Task<ActionResult<object>> TestSecret()
    // {
    //     //await _emailScheduleTask.ExecuteAsync();
    //     await Task.CompletedTask;
    //     return Success();
    // }


    // [HttpPost]
    // [AllowAnonymous]
    // public async Task<ActionResult<object>> AddApeVolo()
    // {
    //     try
    //     {
    //         await _testApeVoloService.CreateAsync(new TestApeVolo
    //         {
    //             Label = "test",
    //             Content = "test",
    //             Sort = 1
    //         });
    //     }
    //     catch
    //     {
    //         // ignored
    //     }
    //
    //     return Success();
    // }

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

    // [HttpGet]
    // [AllowAnonymous]
    // public async Task<ActionResult<object>> TestRedisMq()
    // {
    //     try
    //     {
    //         await _redisCacheService.ListLeftPushAsync(MqTopicNameKey.MailboxQueue, "123456789");
    //     }
    //     catch
    //     {
    //         // ignored
    //     }
    //
    //     return Success();
    // }


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
}
