using System.ComponentModel;
using System.Threading.Tasks;
using ApeVolo.Api.ActionExtension.Json;
using ApeVolo.Api.Controllers.Base;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Model;
using ApeVolo.IBusiness.Dto.Queued.Email;
using ApeVolo.IBusiness.Interface.Queued.Email;
using ApeVolo.IBusiness.QueryModel;
using ApeVolo.IBusiness.RequestModel;
using Microsoft.AspNetCore.Mvc;

namespace ApeVolo.Api.Controllers.Queued.Email;

/// <summary>
/// 邮箱账户
/// </summary>
[Area("Queued")]
[Route("/api/queued/email")]
public class QueuedEmailController : BaseApiController
{
    private readonly IQueuedEmailService _queuedEmailService;

    public QueuedEmailController(IQueuedEmailService queuedEmailService)
    {
        _queuedEmailService = queuedEmailService;
    }


    /// <summary>
    /// 新增邮箱账户
    /// </summary>
    /// <param name="createUpdateQueuedEmailDto"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("create")]
    [Description("{0}Add")]
    public async Task<ActionResult<object>> Create(
        [FromBody] CreateUpdateQueuedEmailDto createUpdateQueuedEmailDto)
    {
        await _queuedEmailService.CreateAsync(createUpdateQueuedEmailDto);
        return Create();
    }

    /// <summary>
    /// 更新邮箱账户
    /// </summary>
    /// <param name="createUpdateQueuedEmailDto"></param>
    /// <returns></returns>
    [HttpPut]
    [Route("edit")]
    [Description("{0}Edit")]
    public async Task<ActionResult<object>> Update(
        [FromBody] CreateUpdateQueuedEmailDto createUpdateQueuedEmailDto)
    {
        await _queuedEmailService.UpdateAsync(createUpdateQueuedEmailDto);
        return NoContent();
    }

    /// <summary>
    /// 删除邮箱账户
    /// </summary>
    /// <param name="idCollection"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route("delete")]
    [Description("{0}Delete")]
    public async Task<ActionResult<object>> Delete([FromBody] IdCollection idCollection)
    {
        if (!ModelState.IsValid)
        {
            var actionError = ModelState.GetErrors();
            return Error(actionError);
        }

        await _queuedEmailService.DeleteAsync(idCollection.IdArray);
        return Success();
    }

    /// <summary>
    /// 邮箱账户列表
    /// </summary>
    /// <param name="queuedEmailQueryCriteria"></param>
    /// <param name="pagination"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("query")]
    [Description("{0}List")]
    public async Task<ActionResult<object>> Query(QueuedEmailQueryCriteria queuedEmailQueryCriteria,
        Pagination pagination)
    {
        var queuedEmailDtoList = await _queuedEmailService.QueryAsync(queuedEmailQueryCriteria, pagination);


        return new ActionResultVm<QueuedEmailDto>
        {
            Content = queuedEmailDtoList,
            TotalElements = pagination.TotalElements
        }.ToJson();
    }
}