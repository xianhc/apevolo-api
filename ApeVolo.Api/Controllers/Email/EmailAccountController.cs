﻿using ApeVolo.Api.Controllers.Base;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Helper;
using ApeVolo.Common.Helper.Excel;
using ApeVolo.Common.Model;
using ApeVolo.IBusiness.Dto.Email;
using ApeVolo.IBusiness.EditDto.Email;
using ApeVolo.IBusiness.Interface.Email;
using ApeVolo.IBusiness.QueryModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using ApeVolo.Api.ActionExtension.Json;

namespace ApeVolo.Api.Controllers.Email
{
    /// <summary>
    /// 邮箱账户
    /// </summary>
    [Area("emailAccount")]
    [Route("/api/email/account")]
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
        [Description("新增邮箱账户")]
        public async Task<ActionResult<object>> Create(
            [FromBody] CreateUpdateEmailAccountDto createUpdateEmailAccountDto)
        {
            RequiredHelper.IsValid(createUpdateEmailAccountDto);
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
        [Description("更新邮箱账户")]
        public async Task<ActionResult<object>> Update(
            [FromBody] CreateUpdateEmailAccountDto createUpdateEmailAccountDto)
        {
            RequiredHelper.IsValid(createUpdateEmailAccountDto);
            await _emailAccountService.UpdateAsync(createUpdateEmailAccountDto);
            return NoContent();
        }

        /// <summary>
        /// 删除邮箱账户
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("delete")]
        [Description("删除邮箱账户")]
        [NoJsonParamter]
        public async Task<ActionResult<object>> Delete([FromBody] HashSet<string> ids)
        {
            if (ids.IsNull() || ids.Count <= 0)
                return Error("ids is null");
            await _emailAccountService.DeleteAsync(ids);
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
        [Description("邮箱账户列表")]
        public async Task<ActionResult<object>> FindList(EmailAccountQueryCriteria emailAccountQueryCriteria,
            Pagination pagination)
        {
            var emailAccountList = await _emailAccountService.QueryAsync(emailAccountQueryCriteria, pagination);


            return new ActionResultVm<EmailAccountDto>()
            {
                Content = emailAccountList,
                TotalElements = pagination.TotalElements
            }.ToJson();
        }

        /// <summary>
        /// 导出邮箱账户
        /// </summary>
        /// <param name="emailAccountQueryCriteria"></param>
        /// <returns></returns>
        [HttpGet]
        [Description("导出邮箱账户")]
        [Route("download")]
        public async Task<ActionResult<object>> Download(EmailAccountQueryCriteria emailAccountQueryCriteria)
        {
            var exportRowModels = await _emailAccountService.DownloadAsync(emailAccountQueryCriteria);

            var filepath = ExcelHelper.ExportData(exportRowModels, "邮箱账户列表");

            var provider = new FileExtensionContentTypeProvider();
            FileInfo fileInfo = new FileInfo(filepath);
            var ext = fileInfo.Extension;
            new FileExtensionContentTypeProvider().Mappings.TryGetValue(ext, out var contently);
            return File(await System.IO.File.ReadAllBytesAsync(filepath), contently ?? "application/octet-stream",
                fileInfo.Name);
        }
    }
}