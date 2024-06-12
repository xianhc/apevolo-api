using System.ComponentModel;
using System.Threading.Tasks;
using Ape.Volo.Api.Controllers.Base;
using Ape.Volo.Common.Extensions;
using Ape.Volo.Common.Helper;
using Ape.Volo.Common.Model;
using Ape.Volo.IBusiness.Dto.Message.Email;
using Ape.Volo.IBusiness.Interface.Message.Email;
using Ape.Volo.IBusiness.QueryModel;
using Ape.Volo.IBusiness.RequestModel;
using Microsoft.AspNetCore.Mvc;

namespace Ape.Volo.Api.Controllers.Message.Email;

/// <summary>
/// 邮箱账户管理
/// </summary>
[Area("邮箱账户管理")]
[Route("/api/email/account", Order = 17)]
public class EmailAccountController : BaseApiController
{
    private readonly IEmailAccountService _emailAccountService;

    public EmailAccountController(IEmailAccountService emailAccountService)
    {
        _emailAccountService = emailAccountService;
    }


    /// <summary>
    /// 新增邮箱账户
    /// </summary>
    /// <param name="createUpdateEmailAccountDto"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("create")]
    [Description("增加")]
    public async Task<ActionResult<object>> Create(
        [FromBody] CreateUpdateEmailAccountDto createUpdateEmailAccountDto)
    {
        if (!ModelState.IsValid)
        {
            var actionError = ModelState.GetErrors();
            return Error(actionError);
        }

        await _emailAccountService.CreateAsync(createUpdateEmailAccountDto);
        return Create();
    }

    /// <summary>
    /// 更新邮箱账户
    /// </summary>
    /// <param name="createUpdateEmailAccountDto"></param>
    /// <returns></returns>
    [HttpPut]
    [Route("edit")]
    [Description("编辑")]
    public async Task<ActionResult<object>> Update(
        [FromBody] CreateUpdateEmailAccountDto createUpdateEmailAccountDto)
    {
        if (!ModelState.IsValid)
        {
            var actionError = ModelState.GetErrors();
            return Error(actionError);
        }

        await _emailAccountService.UpdateAsync(createUpdateEmailAccountDto);
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

        await _emailAccountService.DeleteAsync(idCollection.IdArray);
        return Success();
    }

    /// <summary>
    /// 邮箱账户列表
    /// </summary>
    /// <param name="emailAccountQueryCriteria"></param>
    /// <param name="pagination"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("query")]
    [Description("列表")]
    public async Task<ActionResult<object>> Query(EmailAccountQueryCriteria emailAccountQueryCriteria,
        Pagination pagination)
    {
        var emailAccountList = await _emailAccountService.QueryAsync(emailAccountQueryCriteria, pagination);


        return JsonContent(new ActionResultVm<EmailAccountDto>
        {
            Content = emailAccountList,
            TotalElements = pagination.TotalElements
        });
    }

    /// <summary>
    /// 导出邮箱账户
    /// </summary>
    /// <param name="emailAccountQueryCriteria"></param>
    /// <returns></returns>
    [HttpGet]
    [Description("导出")]
    [Route("download")]
    public async Task<ActionResult<object>> Download(EmailAccountQueryCriteria emailAccountQueryCriteria)
    {
        var emailAccountExports = await _emailAccountService.DownloadAsync(emailAccountQueryCriteria);
        var data = new ExcelHelper().GenerateExcel(emailAccountExports, out var mimeType);
        return File(data, mimeType);
    }
}
