using System.ComponentModel;
using System.Threading.Tasks;
using Ape.Volo.Api.Controllers.Base;
using Ape.Volo.Common.Attributes;
using Ape.Volo.Common.Extensions;
using Ape.Volo.IBusiness.Interface.Monitor;
using Microsoft.AspNetCore.Mvc;

namespace Ape.Volo.Api.Controllers.Monitor;

/// <summary>
/// 服务器管理
/// </summary>
[Area("服务器管理")]
[Route("/api/service", Order = 16)]
public class ServerResourcesController : BaseApiController
{
    private readonly IServerResourcesService _serverResourcesService;

    public ServerResourcesController(IServerResourcesService serverResourcesService)
    {
        _serverResourcesService = serverResourcesService;
    }

    #region 对内接口

    [HttpGet]
    [Route("resources/info")]
    [Description("服务器信息")]
    [NotAudit]
    public async Task<ActionResult<object>> Query()
    {
        var resourcesInfo = await _serverResourcesService.Query();

        return resourcesInfo.ToJson();
    }

    #endregion
}
