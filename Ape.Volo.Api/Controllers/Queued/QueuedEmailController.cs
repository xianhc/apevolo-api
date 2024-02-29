using System.ComponentModel;
using System.Threading.Tasks;
using Ape.Volo.Api.Controllers.Base;
using Ape.Volo.Common.Extention;
using Ape.Volo.Common.Model;
using Ape.Volo.IBusiness.Dto.Queued;
using Ape.Volo.IBusiness.Interface.Queued;
using Ape.Volo.IBusiness.QueryModel;
using Ape.Volo.IBusiness.RequestModel;
using Microsoft.AspNetCore.Mvc;

namespace Ape.Volo.Api.Controllers.Queued;

/// <summary>
/// 邮件队列管理
/// </summary>
[Area("邮件队列管理")]
[Route("/api/queued/email", Order = 19)]
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
    [Description("创建")]
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
    [Description("编辑")]
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
    [Description("删除")]
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
    [Description("查询")]
    public async Task<ActionResult<object>> Query(QueuedEmailQueryCriteria queuedEmailQueryCriteria,
        Pagination pagination)
    {
        var queuedEmailDtoList = await _queuedEmailService.QueryAsync(queuedEmailQueryCriteria, pagination);


        return JsonContent(new ActionResultVm<QueuedEmailDto>
        {
            Content = queuedEmailDtoList,
            TotalElements = pagination.TotalElements
        });
    }
}
