using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using ApeVolo.Api.ActionExtension.Json;
using ApeVolo.Api.Controllers.Base;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Helper.Excel;
using ApeVolo.Common.Model;
using ApeVolo.IBusiness.Dto.Core;
using ApeVolo.IBusiness.EditDto.Core;
using ApeVolo.IBusiness.Interface.Core;
using ApeVolo.IBusiness.QueryModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace ApeVolo.Api.Controllers
{
    /// <summary>
    /// 全局设置管理
    /// </summary>
    [Area("AppSecret")]
    [Route("/api/appSecret")]
    public class AppSecretController : BaseApiController
    {
        #region 字段

        private readonly IAppSecretService _appSecretService;

        #endregion

        #region 构造函数

        public AppSecretController(IAppSecretService appSecretService)
        {
            _appSecretService = appSecretService;
        }

        #endregion

        #region 内部接口

        /// <summary>
        /// 新增设置
        /// </summary>
        /// <param name="createUpdateAppSecretDto"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("create")]
        [Description("新增应用秘钥")]
        public async Task<ActionResult<object>> Create(
            [FromBody] CreateUpdateAppSecretDto createUpdateAppSecretDto)
        {
            await _appSecretService.CreateAsync(createUpdateAppSecretDto);
            return Create();
        }

        /// <summary>
        /// 更新设置
        /// </summary>
        /// <param name="createUpdateAppSecretDto"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("edit")]
        [Description("更新应用秘钥")]
        public async Task<ActionResult<object>> Update(
            [FromBody] CreateUpdateAppSecretDto createUpdateAppSecretDto)
        {
            await _appSecretService.UpdateAsync(createUpdateAppSecretDto);
            return NoContent();
        }

        /// <summary>
        /// 删除设置
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("delete")]
        [Description("删除应用秘钥")]
        [NoJsonParamter]
        public async Task<ActionResult<object>> Delete([FromBody] HashSet<long> ids)
        {
            if (ids == null || ids.Count < 1)
            {
                return Error("ids is null");
            }

            await _appSecretService.DeleteAsync(ids);
            return Success();
        }

        /// <summary>
        /// 获取设置列表
        /// </summary>
        /// <param name="appsecretQueryCriteria"></param>
        /// <param name="pagination"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("query")]
        [Description("获取应用秘钥列表")]
        public async Task<ActionResult<object>> Query(AppsecretQueryCriteria appsecretQueryCriteria,
            Pagination pagination)
        {
            var appSecretList = await _appSecretService.QueryAsync(appsecretQueryCriteria, pagination);

            return new ActionResultVm<AppSecretDto>
            {
                Content = appSecretList,
                TotalElements = pagination.TotalElements
            }.ToJson();
        }


        /// <summary>
        /// 导出设置
        /// </summary>
        /// <param name="appsecretQueryCriteria"></param>
        /// <returns></returns>
        [HttpGet]
        [Description("导出应用秘钥")]
        [Route("download")]
        public async Task<ActionResult<object>> Download(AppsecretQueryCriteria appsecretQueryCriteria)
        {
            var exportRowModels = await _appSecretService.DownloadAsync(appsecretQueryCriteria);

            var filepath = ExcelHelper.ExportData(exportRowModels, "应用秘钥");

            var provider = new FileExtensionContentTypeProvider();
            FileInfo fileInfo = new FileInfo(filepath);
            var ext = fileInfo.Extension;
            new FileExtensionContentTypeProvider().Mappings.TryGetValue(ext, out var contently);
            return File(await System.IO.File.ReadAllBytesAsync(filepath), contently ?? "application/octet-stream",
                fileInfo.Name);
        }

        #endregion
    }
}