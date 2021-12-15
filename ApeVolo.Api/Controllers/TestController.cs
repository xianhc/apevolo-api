using System.Collections.Generic;
using System.Threading.Tasks;
using ApeVolo.Api.Controllers.Base;
using ApeVolo.Common.Caches.Redis.Service;
using ApeVolo.Common.Caches.Redis.Service.MessageQueue;
using ApeVolo.Common.Extention;
using ApeVolo.Common.WebApp;
using ApeVolo.Entity.Do.Core;
using ApeVolo.IBusiness.EditDto.Core;
using ApeVolo.IBusiness.Interface.Core;
using ApeVolo.IBusiness.Interface.Email;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApeVolo.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    //[ApiController]
    //[Authorize(Policy = GlobalSwitch.AUTH_POLICYS_NAME)]
    public class TestController : BaseApiController
    {
        private readonly ICurrentUser _currentUser;
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IUserRolesService _userRolesService;
        private readonly IDepartmentService _departmentService;
        private readonly IMenuService _menuService;
        private readonly IAppSecretService _appSecretService;
        private readonly IEmailScheduleTask _emailScheduleTask;
        private readonly ITestApeVoloService _testApeVoloService;
        private readonly IRedisCacheService _redisCacheService;

        public TestController(ICurrentUser currentUser, IUserService userService, IRoleService roleService,
            IUserRolesService userRolesService, IDepartmentService departmentService, IMenuService menuService,
            IAppSecretService appSecretService, IEmailScheduleTask emailScheduleTask,
            ITestApeVoloService testApeVoloService, IRedisCacheService redisCacheService)
        {
            _currentUser = currentUser;
            _userService = userService;
            _roleService = roleService;
            _userRolesService = userRolesService;
            _departmentService = departmentService;
            _menuService = menuService;
            _appSecretService = appSecretService;
            _emailScheduleTask = emailScheduleTask;
            _testApeVoloService = testApeVoloService;
            _redisCacheService = redisCacheService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<object>> TestUserLevel()
        {
            var isTrue = await _userRolesService.DeleteByUserIdAsync(789739052218716160);


            var level = await _roleService.QueryUserRoleLevelAsync(null);
            Dictionary<string, int> keyValuePairs = new Dictionary<string, int> {{"level", level}};
            return keyValuePairs.ToJson();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<object>> TestSecret()
        {
            await _emailScheduleTask.ExecuteAsync();
            var text = await _appSecretService.CreateAsync(new CreateUpdateAppSecretDto {AppName = "test"});
            return text.ToJson();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<object>> AddApeVolo()
        {
            await _testApeVoloService.CreateAsync(new TestApeVolo
            {
                Label = "test",
                Content = "test",
                Sort = 1
            });
            return Success();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<object>> TestRedisMQ()
        {
            await _redisCacheService.ListLeftPushAsync(MqTopicNameKey.MailboxQueue, "123456789");
            //await _redisCacheService.SetCacheAsync("qwer", "123");
            var text = "true";
            return text.ToJson();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<object>> TestConModels()
        {
            HashSet<long> ids = new HashSet<long>();
            ids.Add(1306054134645919750);
            ids.Add(123456);

            List<long> ids2 = new List<long>();
            ids2.Add(1306054134645919750);
            ids2.Add(123456);

            var user = await _userService.QueryByIdsAsync(ids);
            var user2 = await _userService.QueryByIdsAsync(ids2);
            var text = "true";
            return text.ToJson();
        }
    }
}