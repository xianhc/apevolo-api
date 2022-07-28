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

namespace ApeVolo.Api.Controllers.Email;

/// <summary>
/// 邮箱账户
/// </summary>
[Area("emailMessageTemplate")]
[Route("/api/email/template")]
public class EmailMessageTemplateController : BaseApiController
{
    #region 字段

    private readonly IEmailMessageTemplateService _emailMessageTemplateService;

    #endregion

    #region 构造函数

    public EmailMessageTemplateController(IEmailMessageTemplateService emailMessageTemplateService)
    {
        _emailMessageTemplateService = emailMessageTemplateService;
    }

    #endregion

    #region API

    /// <summary>
    /// 新增邮箱账户
    /// </summary>
    /// <param name="createUpdateEmailMessageTemplateDto"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("create")]
    [Description("新增邮件消息模板")]
    public async Task<ActionResult<object>> Create(
        [FromBody] CreateUpdateEmailMessageTemplateDto createUpdateEmailMessageTemplateDto)
    {
        RequiredHelper.IsValid(createUpdateEmailMessageTemplateDto);
        await _emailMessageTemplateService.CreateAsync(createUpdateEmailMessageTemplateDto);
        return Create();
    }

    /// <summary>
    /// 更新邮箱账户
    /// </summary>
    /// <param name="createUpdateEmailMessageTemplateDto"></param>
    /// <returns></returns>
    [HttpPut]
    [Route("edit")]
    [Description("更新邮件消息模板")]
    public async Task<ActionResult<object>> Update(
        [FromBody] CreateUpdateEmailMessageTemplateDto createUpdateEmailMessageTemplateDto)
    {
        RequiredHelper.IsValid(createUpdateEmailMessageTemplateDto);
        await _emailMessageTemplateService.UpdateAsync(createUpdateEmailMessageTemplateDto);
        return NoContent();
    }

    /// <summary>
    /// 删除邮箱账户
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route("delete")]
    [Description("删除邮件消息模板")]
    [NoJsonParamter]
    public async Task<ActionResult<object>> Delete([FromBody] HashSet<long> ids)
    {
        if (ids.IsNull() || ids.Count <= 0)
            return Error("ids is null");
        await _emailMessageTemplateService.DeleteAsync(ids);
        return Success();
    }

    /// <summary>
    /// 邮箱账户列表
    /// </summary>
    /// <param name="messageTemplateQueryCriteria"></param>
    /// <param name="pagination"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("query")]
    [Description("邮件消息模板列表")]
    public async Task<ActionResult<object>> Query(EmailMessageTemplateQueryCriteria messageTemplateQueryCriteria,
        Pagination pagination)
    {
        var emailMessageTemplateList =
            await _emailMessageTemplateService.QueryAsync(messageTemplateQueryCriteria, pagination);

        return new ActionResultVm<EmailMessageTemplateDto>
        {
            Content = emailMessageTemplateList,
            TotalElements = pagination.TotalElements
        }.ToJson();
    }

    #endregion
}