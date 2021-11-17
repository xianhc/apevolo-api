using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using ApeVolo.Api.Controllers.Base;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Model;
using ApeVolo.IBusiness.Interface.Dictionary;
using ApeVolo.IBusiness.Dto.Dictionary;
using ApeVolo.IBusiness.EditDto.Dict;
using ApeVolo.IBusiness.QueryModel;
using Microsoft.AspNetCore.Mvc;
using ApeVolo.Common.Helper.Excel;
using Microsoft.AspNetCore.StaticFiles;
using System.IO;
using ApeVolo.Api.ActionExtension.Json;

namespace ApeVolo.Api.Controllers
{
    /// <summary>
    /// 字典管理
    /// </summary>
    [Area("Dict")]
    [Route("/api/dict")]
    public class DictController : BaseApiController
    {
        #region 字段

        private readonly IDictService _dictService;

        #endregion

        #region 构造函数

        public DictController(IDictService dictService)
        {
            _dictService = dictService;
        }

        #endregion

        #region 内部接口

        /// <summary>
        /// 新增字典
        /// </summary>
        /// <param name="createUpdateDictDto"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("create")]
        [Description("新增字典")]
        public async Task<ActionResult<object>> Create([FromBody] CreateUpdateDictDto createUpdateDictDto)
        {
            await _dictService.CreateAsync(createUpdateDictDto);
            return Success();
        }


        /// <summary>
        /// 更新字典
        /// </summary>
        /// <param name="createUpdateDictDto"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("edit")]
        [Description("更新字典")]
        public async Task<ActionResult<object>> Update([FromBody] CreateUpdateDictDto createUpdateDictDto)
        {
            await _dictService.UpdateAsync(createUpdateDictDto);
            return NoContent();
        }

        /// <summary>
        /// 删除字典
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("delete")]
        [Description("删除字典")]
        [NoJsonParamter]
        public async Task<ActionResult<object>> Delete([FromBody] HashSet<string> ids)
        {
            if (ids == null || ids.Count < 1)
            {
                return Error("ids is null");
            }

            await _dictService.DeleteAsync(ids);
            return Success();
        }

        /// <summary>
        /// 查看字典列表
        /// </summary>
        /// <param name="dictQueryCriteria"></param>
        /// <param name="pagination"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("query")]
        [Description("查看字典列表")]
        public async Task<ActionResult<object>> Query(DictQueryCriteria dictQueryCriteria,
            Pagination pagination)
        {
            var list = await _dictService.QueryAsync(dictQueryCriteria, pagination);

            return new ActionResultVm<DictDto>()
            {
                Content = list,
                TotalElements = pagination.TotalElements
            }.ToJson();
        }

        /// <summary>
        /// 导出字典
        /// </summary>
        /// <param name="dictQueryCriteria"></param>
        /// <returns></returns>
        [HttpGet]
        [Description("导出字典")]
        [Route("download")]
        public async Task<ActionResult<object>> Download(DictQueryCriteria dictQueryCriteria)
        {
            var exportRowModels = await _dictService.DownloadAsync(dictQueryCriteria);

            var filepath = ExcelHelper.ExportData(exportRowModels, "字典列表");

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