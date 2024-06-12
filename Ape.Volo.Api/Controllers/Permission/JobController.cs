using System.ComponentModel;
using System.Threading.Tasks;
using Ape.Volo.Api.Controllers.Base;
using Ape.Volo.Common.Extensions;
using Ape.Volo.Common.Helper;
using Ape.Volo.Common.Model;
using Ape.Volo.IBusiness.Dto.Permission;
using Ape.Volo.IBusiness.Interface.Permission;
using Ape.Volo.IBusiness.QueryModel;
using Ape.Volo.IBusiness.RequestModel;
using Microsoft.AspNetCore.Mvc;

namespace Ape.Volo.Api.Controllers.Permission;

/// <summary>
/// 岗位管理
/// </summary>
[Area("岗位管理")]
[Route("/api/job", Order = 6)]
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
    [Description("创建")]
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
    [Description("编辑")]
    public async Task<ActionResult<object>> Update(
        [FromBody] CreateUpdateJobDto createUpdateJobDto)
    {
        if (!ModelState.IsValid)
        {
            var actionError = ModelState.GetErrors();
            return Error(actionError);
        }

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
    [Description("删除")]
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
    [Description("查询")]
    public async Task<ActionResult<object>> Query(JobQueryCriteria jobQueryCriteria, Pagination pagination)
    {
        var jobList = await _jobService.QueryAsync(jobQueryCriteria, pagination);
        return JsonContent(new ActionResultVm<JobDto>
        {
            Content = jobList,
            TotalElements = pagination.TotalElements
        });
    }

    /// <summary>
    /// 获取所有岗位
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("queryAll")]
    [Description("查询全部")]
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
    [Description("导出")]
    [Route("download")]
    public async Task<ActionResult<object>> Download(JobQueryCriteria jobQueryCriteria)
    {
        var jobExports = await _jobService.DownloadAsync(jobQueryCriteria);
        var data = new ExcelHelper().GenerateExcel(jobExports, out var mimeType);
        return File(data, mimeType);
    }

    #endregion
}
