using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using ApeVolo.Api.Controllers.Base;
using ApeVolo.Common.AttributeExt;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Model;
using ApeVolo.IBusiness.Dto.Monitor.Logs.Exception;
using ApeVolo.IBusiness.Interface.Monitor.Exception;
using ApeVolo.IBusiness.QueryModel;
using Microsoft.AspNetCore.Mvc;

namespace ApeVolo.Api.Controllers.Monitor.Exception;

/// <summary>
/// 系统异常日志管理
/// </summary>
[Area("Exception")]
[Route("/api/exception")]
public class ExceptionLogController : BaseApiController
{
    #region 字段

    private readonly IExceptionLogService _exceptionLogService;

    #endregion

    #region 构造函数

    public ExceptionLogController(IExceptionLogService exceptionLogService)
    {
        _exceptionLogService = exceptionLogService;
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
    [Description("{0}List")]
    public async Task<ActionResult<object>> Query(LogQueryCriteria logQueryCriteria,
        Pagination pagination)
    {
        var exceptionLogs = await _exceptionLogService.QueryAsync(logQueryCriteria, pagination);

        return new ActionResultVm<ExceptionLogDto>
        {
            Content = exceptionLogs,
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
    [Description("LogStack")]
    [ApeVoloAuthorize(new[] { "admin", "log_list" })]
    public async Task<ActionResult<object>> QueryDetail(string id)
    {
        if (id.IsNullOrEmpty())
        {
            return Error("id is null");
        }

        var log = await _exceptionLogService.QuerySingleAsync(id);
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