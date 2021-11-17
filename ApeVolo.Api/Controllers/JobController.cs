using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using ApeVolo.Api.Controllers.Base;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Model;
using ApeVolo.IBusiness.Interface.Core;
using ApeVolo.IBusiness.Dto.Core;
using ApeVolo.IBusiness.EditDto.Core;
using ApeVolo.IBusiness.QueryModel;
using Microsoft.AspNetCore.Mvc;
using ApeVolo.Common.Helper.Excel;
using Microsoft.AspNetCore.StaticFiles;
using System.IO;
using ApeVolo.Api.ActionExtension.Json;
using ApeVolo.Common.AttributeExt;

namespace ApeVolo.Api.Controllers
{
    /// <summary>
    /// 岗位管理
    /// </summary>
    [Area("Job")]
    [Route("/api/job")]
    public class JobController : BaseApiController
    {
        #region 字段

        private readonly IJobService _jobService;

        #endregion

        #region 构造函数

        public JobController(IJobService jobService)
        {
            _jobService = jobService;
        }

        #endregion

        #region 内部接口

        /// <summary>
        /// 新增岗位
        /// </summary>
        /// <param name="createUpdateJobDto"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("create")]
        [Description("新增岗位")]
        public async Task<ActionResult<object>> Create(
            [FromBody] CreateUpdateJobDto createUpdateJobDto)
        {
            await _jobService.CreateAsync(createUpdateJobDto);
            return Create();
        }

        /// <summary>
        /// 更新岗位
        /// </summary>
        /// <param name="createUpdateJobDto"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("edit")]
        [Description("更新岗位")]
        public async Task<ActionResult<object>> Update(
            [FromBody] CreateUpdateJobDto createUpdateJobDto)
        {
            await _jobService.UpdateAsync(createUpdateJobDto);
            return NoContent();
        }

        /// <summary>
        /// 删除岗位
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("delete")]
        [Description("删除岗位")]
        [NoJsonParamter]
        public async Task<ActionResult<object>> Delete([FromBody] HashSet<string> ids)
        {
            if (ids == null || ids.Count < 1)
            {
                return Error("ids is null");
            }

            await _jobService.DeleteAsync(ids);
            return Success();
        }

        /// <summary>
        /// 查看岗位列表
        /// </summary>
        /// <param name="jobQueryCriteria"></param>
        /// <param name="pagination"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("query")]
        [Description("获取岗位列表")]
        public async Task<ActionResult<object>> Query(JobQueryCriteria jobQueryCriteria, Pagination pagination)
        {
            var jobList = await _jobService.QueryAsync(jobQueryCriteria, pagination);

            return new ActionResultVm<JobDto>()
            {
                Content = jobList,
                TotalElements = pagination.TotalElements
            }.ToJson();
        }

        /// <summary>
        /// 获取所有岗位
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("queryAll")]
        [Description("获取岗位列表")]
        [ApeVoloAuthorize(new[] {"admin", "job_list"})]
        public async Task<ActionResult<object>> QueryAll()
        {
            var jobList = await _jobService.QueryAllAsync();

            return new ActionResultVm<JobDto>()
            {
                Content = jobList,
                TotalElements = jobList.Count
            }.ToJson();
        }

        /// <summary>
        /// 导出岗位
        /// </summary>
        /// <param name="jobQueryCriteria"></param>
        /// <returns></returns>
        [HttpGet]
        [Description("导出岗位")]
        [Route("download")]
        public async Task<ActionResult<object>> Download(JobQueryCriteria jobQueryCriteria)
        {
            var exportRowModels = await _jobService.DownloadAsync(jobQueryCriteria);

            var filepath = ExcelHelper.ExportData(exportRowModels, "岗位列表");

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