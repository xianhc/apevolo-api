using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using ApeVolo.Api.Controllers.Base;
using ApeVolo.Common.AttributeExt;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Model;
using ApeVolo.IBusiness.Dto.Logs;
using ApeVolo.IBusiness.Interface.Logs;
using ApeVolo.IBusiness.QueryModel;
using Microsoft.AspNetCore.Mvc;

namespace ApeVolo.Api.Controllers;

/// <summary>
/// 系统异常日志管理
/// </summary>
[Area("SystemLog")]
[Route("/api/log")]
public class LogController : BaseApiController
{
    #region 字段

    private readonly ILogService _logService;

    #endregion

    #region 构造函数

    public LogController(ILogService logService)
    {
        _logService = logService;
    }

    #endregion

    #region 内部接口

    /// <summary>
    /// 查看异常日志列表
    /// </summary>
    /// <param name="logQueryCriteria"></param>
    /// <param name="pagination"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("query")]
    [Description("查看异常日志列表")]
    public async Task<ActionResult<object>> FindList(LogQueryCriteria logQueryCriteria,
        Pagination pagination)
    {
        var auditInfos = await _logService.QueryAsync(logQueryCriteria, pagination);

        return new ActionResultVm<LogDto>
        {
            Content = auditInfos,
            TotalElements = pagination.TotalElements
        }.ToJson();
    }

    /// <summary>
    /// 查看日志堆栈详情
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("detail/{id}")]
    [Description("查看日志堆栈详情")]
    [ApeVoloAuthorize(new[] { "admin", "log_list" })]
    public async Task<ActionResult<object>> QueryDetail(string id)
    {
        if (id.IsNullOrEmpty())
        {
            return Error("id is null");
        }

        var log = await _logService.QuerySingleAsync(id);
        Dictionary<string, string> logDetail = new Dictionary<string, string>
        {
            {
                "ExceptionInfoFull",
                log.RequestUrl + "\r\n" +
                log.RequestParameters + "\r\n" +
                //log.ExceptionMessage + "\r\n" +
                log.ExceptionMessageFull + "\r\n" +
                log.ExceptionStack
            }
        };
        return logDetail.ToJson();
    }

    #endregion
}