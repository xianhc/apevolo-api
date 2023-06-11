using System.ComponentModel;
using System.Threading.Tasks;
using ApeVolo.Api.Controllers.Base;
using ApeVolo.Common.AttributeExt;
using ApeVolo.Common.Extention;
using ApeVolo.IBusiness.Interface.Monitor.Server;
using Microsoft.AspNetCore.Mvc;

namespace ApeVolo.Api.Controllers.Monitor.Server;

/// <summary>
/// 服务器监控
/// </summary>
[Area("Monitor")]
[Route("/api/")]
public class ServerResourcesController : BaseApiController
{
    private readonly IServerResourcesService _serverResourcesService;

    public ServerResourcesController(IServerResourcesService serverResourcesService)
    {
        _serverResourcesService = serverResourcesService;
    }

    #region 对内接口

    [HttpGet]
    [Route("service/resources/info")]
    [Description("Info")]
    [ApeVoloAuthorize(new[] { "admin" })]
    public async Task<ActionResult<object>> Query()
    {
        var resourcesInfo = await _serverResourcesService.Query();

        return resourcesInfo.ToJson();
    }

    #endregion
}