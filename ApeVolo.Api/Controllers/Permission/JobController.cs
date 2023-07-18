using System.ComponentModel;
using System.Threading.Tasks;
using ApeVolo.Api.Controllers.Base;
using ApeVolo.Common.AttributeExt;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Helper;
using ApeVolo.Common.Model;
using ApeVolo.IBusiness.Dto.Permission;
using ApeVolo.IBusiness.Interface.Permission;
using ApeVolo.IBusiness.QueryModel;
using ApeVolo.IBusiness.RequestModel;
using Microsoft.AspNetCore.Mvc;

namespace ApeVolo.Api.Controllers.Permission;

/// <summary>
/// 岗位管理
/// </summary>
[Area("Permission")]
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
    [Description("Add")]
    public async Task<ActionResult<object>> Create(
        [FromBody] CreateUpdateJobDto createUpdateJobDto)
    {
        if (!ModelState.IsValid)
        {
            var actionError = ModelState.GetErrors();
            return Error(actionError);
        }

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
    [Description("Edit")]
    public async Task<ActionResult<object>> Update(
        [FromBody] CreateUpdateJobDto createUpdateJobDto)
    {
        await _jobService.UpdateAsync(createUpdateJobDto);
        return NoContent();
    }

    /// <summary>
    /// 删除岗位
    /// </summary>
    /// <param name="idCollection"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route("delete")]
    [Description("Delete")]
    public async Task<ActionResult<object>> Delete([FromBody] IdCollection idCollection)
    {
        if (!ModelState.IsValid)
        {
            var actionError = ModelState.GetErrors();
            return Error(actionError);
        }

        await _jobService.DeleteAsync(idCollection.IdArray);
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
    [Description("List")]
    public async Task<ActionResult<object>> Query(JobQueryCriteria jobQueryCriteria, Pagination pagination)
    {
        var jobList = await _jobService.QueryAsync(jobQueryCriteria, pagination);

        return new ActionResultVm<JobDto>
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
    [Description("List")]
    [ApeVoloAuthorize(new[] { "admin", "job_list" })]
    public async Task<ActionResult<object>> QueryAll()
    {
        var jobList = await _jobService.QueryAllAsync();

        return new ActionResultVm<JobDto>
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
    [Description("Export")]
    [Route("download")]
    public async Task<ActionResult<object>> Download(JobQueryCriteria jobQueryCriteria)
    {
        var jobExports = await _jobService.DownloadAsync(jobQueryCriteria);
        var data = new ExcelHelper().GenerateExcel(jobExports, out var mimeType);
        return File(data, mimeType);
    }

    #endregion
}
