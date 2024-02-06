using System.ComponentModel;
using System.Threading.Tasks;
using Ape.Volo.Api.Controllers.Base;
using Ape.Volo.Common.AttributeExt;
using Ape.Volo.Common.Extention;
using Ape.Volo.IBusiness.Interface.Monitor;
using Microsoft.AspNetCore.Mvc;

namespace Ape.Volo.Api.Controllers.Monitor;

/// <summary>
/// 服务器监控
/// </summary>
[Area("监控管理")]
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
    [Description("服务器信息")]
    [ApeVoloAuthorize(new[] { "admin" })]
    [NotAudit]
    public async Task<ActionResult<object>> Query()
    {
        var resourcesInfo = await _serverResourcesService.Query();

        return resourcesInfo.ToJson();
    }

    #endregion
}
