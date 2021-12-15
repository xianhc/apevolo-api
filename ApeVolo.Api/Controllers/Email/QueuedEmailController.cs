using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using ApeVolo.Api.ActionExtension.Json;
using ApeVolo.Api.Controllers.Base;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Helper;
using ApeVolo.Common.Model;
using ApeVolo.IBusiness.Dto.Email;
using ApeVolo.IBusiness.EditDto.Email;
using ApeVolo.IBusiness.Interface.Email;
using ApeVolo.IBusiness.QueryModel;
using Microsoft.AspNetCore.Mvc;

namespace ApeVolo.Api.Controllers.Email
{
    /// <summary>
    /// 邮箱账户
    /// </summary>
    [Area("emailAccount")]
    [Route("/api/email/queued")]
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
        [Description("新增邮件队列")]
        public async Task<ActionResult<object>> Create(
            [FromBody] CreateUpdateQueuedEmailDto createUpdateQueuedEmailDto)
        {
            RequiredHelper.IsValid(createUpdateQueuedEmailDto);
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
        [Description("更新邮件队列")]
        public async Task<ActionResult<object>> Update(
            [FromBody] CreateUpdateQueuedEmailDto createUpdateQueuedEmailDto)
        {
            RequiredHelper.IsValid(createUpdateQueuedEmailDto);
            await _queuedEmailService.UpdateAsync(createUpdateQueuedEmailDto);
            return NoContent();
        }

        /// <summary>
        /// 删除邮箱账户
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("delete")]
        [Description("删除邮件队列")]
        [NoJsonParamter]
        public async Task<ActionResult<object>> Delete([FromBody] HashSet<long> ids)
        {
            if (ids.IsNull() || ids.Count <= 0)
                return Error("ids is null");
            await _queuedEmailService.DeleteAsync(ids);
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
        [Description("邮件队列列表")]
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
}