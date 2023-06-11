using System.ComponentModel;
using System.Threading.Tasks;
using ApeVolo.Api.Controllers.Base;
using ApeVolo.Common.AttributeExt;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Model;
using ApeVolo.Common.WebApp;
using ApeVolo.IBusiness.Dto.Monitor.Logs.Auditing;
using ApeVolo.IBusiness.Interface.Monitor.Auditing;
using ApeVolo.IBusiness.QueryModel;
using Microsoft.AspNetCore.Mvc;

namespace ApeVolo.Api.Controllers.Monitor.Auditing;

/// <summary>
/// 审计管理
/// </summary>
[Area("Monitor")]
[Route("/api/auditing")]
public class AuditingController : BaseApiController
{
    #region 字段

    private readonly IAuditLogService _auditInfoService;
    private readonly ICurrentUser _currentUser;

    #endregion

    #region 构造函数

    public AuditingController(IAuditLogService auditInfoService, ICurrentUser currentUser)
    {
        _auditInfoService = auditInfoService;
        _currentUser = currentUser;
    }

    #endregion

    #region 内部接口

    /// <summary>
    /// 审计列表
    /// </summary>
    /// <param name="logQueryCriteria">查询对象</param>
    /// <param name="pagination">分页对象</param>
    /// <returns></returns>
    [HttpGet]
    [Route("query")]
    [Description("List")]
    public async Task<ActionResult<object>> Query(LogQueryCriteria logQueryCriteria,
        Pagination pagination)
    {
        var auditInfos = await _auditInfoService.QueryAsync(logQueryCriteria, pagination);

        return new ActionResultVm<AuditLogDto>
        {
            Content = auditInfos,
            TotalElements = pagination.TotalElements
        }.ToJson();
    }


    /// <summary>
    /// 当前用户行为
    /// </summary>
    /// <param name="pagination"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("current")]
    [Description("UserBehavior")]
    [ApeVoloOnline]
    public async Task<ActionResult<object>> FindListByCurrent(Pagination pagination)
    {
        var auditInfos = await _auditInfoService.QueryByCurrentAsync(_currentUser.Name, pagination);

        return new ActionResultVm<AuditLogDto>
        {
            Content = auditInfos,
            TotalElements = pagination.TotalElements
        }.ToJson();
    }

    #endregion
}