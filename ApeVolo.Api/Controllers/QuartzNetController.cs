using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using ApeVolo.Api.ActionExtension.Json;
using ApeVolo.Api.Controllers.Base;
using ApeVolo.Common.AttributeExt;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Helper;
using ApeVolo.Common.Helper.Excel;
using ApeVolo.Common.Model;
using ApeVolo.Entity.Do.Tasks;
using ApeVolo.IBusiness.Dto.Tasks;
using ApeVolo.IBusiness.EditDto.Tasks;
using ApeVolo.IBusiness.Interface.Tasks;
using ApeVolo.IBusiness.QueryModel;
using ApeVolo.QuartzNetService.service;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace ApeVolo.Api.Controllers;

/// <summary>
/// 作业调度管理
/// </summary>
[Area("QuartzNet")]
[Route("/api/tasks")]
public class QuartzNetController : BaseApiController
{
    #region 字段

    private readonly IQuartzNetService _quartzNetService;
    private readonly IQuartzNetLogService _quartzNetLogService;
    private readonly ISchedulerCenterService _schedulerCenterService;
    private readonly IMapper _mapper;

    #endregion

    #region 构造函数

    public QuartzNetController(IQuartzNetService quartzNetService, IQuartzNetLogService quartzNetLogService,
        ISchedulerCenterService schedulerCenterService, IMapper mapper)
    {
        _quartzNetService = quartzNetService;
        _quartzNetLogService = quartzNetLogService;
        _schedulerCenterService = schedulerCenterService;
        _mapper = mapper;
    }

    #endregion

    #region 内部接口

    /// <summary>
    /// 新增作业
    /// </summary>
    /// <param name="createUpdateQuartzNetDto"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("create")]
    [Description("新增作业")]
    public async Task<ActionResult<object>> Create(
        [FromBody] CreateUpdateQuartzNetDto createUpdateQuartzNetDto)
    {
        RequiredHelper.IsValid(createUpdateQuartzNetDto);
        createUpdateQuartzNetDto.InitEntity();
        var quartzNet = await _quartzNetService.CreateAsync(createUpdateQuartzNetDto);
        if (quartzNet.IsNotNull())
        {
            string msg = $"创建【{quartzNet.TaskName}】成功";
            if (quartzNet.IsEnable)
            {
                //开启作业任务
                await _schedulerCenterService.AddScheduleJobAsync(quartzNet);
            }

            return Create(msg);
        }

        return Error();
    }

    /// <summary>
    /// 更新作业
    /// </summary>
    /// <param name="createUpdateQuartzNetDto"></param>
    /// <returns></returns>
    [HttpPut]
    [Route("edit")]
    [Description("更新作业")]
    public async Task<ActionResult<object>> Update(
        [FromBody] CreateUpdateQuartzNetDto createUpdateQuartzNetDto)
    {
        RequiredHelper.IsValid(createUpdateQuartzNetDto);
        if (await _quartzNetService.UpdateAsync(createUpdateQuartzNetDto))
        {
            var quartzNet = _mapper.Map<QuartzNet>(createUpdateQuartzNetDto);
            if (quartzNet.IsEnable)
            {
                await _schedulerCenterService.StopScheduleJobAsync(quartzNet);
                await _schedulerCenterService.AddScheduleJobAsync(quartzNet);
            }
            else
            {
                await _schedulerCenterService.StopScheduleJobAsync(quartzNet);
            }

            return NoContent();
        }

        return Error();
    }

    /// <summary>
    /// 删除作业
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route("delete")]
    [Description("删除作业")]
    [NoJsonParamter]
    public async Task<ActionResult<object>> Delete([FromBody] HashSet<long> ids)
    {
        if (ids == null || ids.Count < 1)
            return Error("ids异常");

        var quartzList = await _quartzNetService.QueryByIdsAsync(ids);
        if (quartzList.Count > 0)
        {
            if (await _quartzNetService.DeleteAsync(quartzList))
            {
                foreach (var item in quartzList)
                {
                    await _schedulerCenterService.StopScheduleJobAsync(item);
                }

                return Success();
            }
        }

        return Error();
    }

    /// <summary>
    /// 获取作业调度任务列表
    /// </summary>
    /// <param name="quartzNetQueryCriteria"></param>
    /// <param name="pagination"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("query")]
    [Description("获取作业调度列表")]
    public async Task<ActionResult<object>> Query(QuartzNetQueryCriteria quartzNetQueryCriteria,
        Pagination pagination)
    {
        var quartzNetList = await _quartzNetService.QueryAsync(quartzNetQueryCriteria, pagination);

        foreach (var quartzNet in quartzNetList)
        {
            quartzNet.TriggerStatus = await _schedulerCenterService.GetTriggerStatus(quartzNet);
        }

        return new ActionResultVm<QuartzNetDto>
        {
            Content = quartzNetList,
            TotalElements = pagination.TotalElements
        }.ToJson();
    }

    /// <summary>
    /// 导出作业调度
    /// </summary>
    /// <param name="quartzNetQueryCriteria"></param>
    /// <returns></returns>
    [HttpGet]
    [Description("导出作业调度")]
    [Route("download")]
    public async Task<ActionResult<object>> Download(QuartzNetQueryCriteria quartzNetQueryCriteria)
    {
        var exportRowModels = await _quartzNetService.DownloadAsync(quartzNetQueryCriteria);

        var filepath = ExcelHelper.ExportData(exportRowModels, "作业调度列表");

        var provider = new FileExtensionContentTypeProvider();
        FileInfo fileInfo = new FileInfo(filepath);
        var ext = fileInfo.Extension;
        new FileExtensionContentTypeProvider().Mappings.TryGetValue(ext, out var contently);
        return File(await System.IO.File.ReadAllBytesAsync(filepath), contently ?? "application/octet-stream",
            fileInfo.Name);
    }


    /// <summary>
    /// 执行作业
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPut]
    [Description("执行作业")]
    [Route("execute/{id}")]
    [ApeVoloAuthorize(new[] { "admin" })]
    public async Task<ActionResult<object>> Execute(long id)
    {
        var quartzNet = await _quartzNetService.QueryFirstAsync(x => x.Id == id);
        if (quartzNet.IsNull())
        {
            return Error("作业不存在，执行失败！");
        }

        //开启作业任务
        quartzNet.IsEnable = true;
        if (await _quartzNetService.UpdateEntityAsync(quartzNet))
        {
            //检查任务在内存状态
            var isTrue = await _schedulerCenterService.IsExistScheduleJobAsync(quartzNet);
            if (!isTrue)
            {
                if (await _schedulerCenterService.AddScheduleJobAsync(quartzNet))
                {
                    return NoContent();
                }

                return Error("执行失败，请重试！");
            }

            return Error("作业已在运行，请勿重复开启！");
        }

        return Error("执行DB失败！");
    }

    /// <summary>
    /// 暂停作业
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPut]
    [Description("暂停作业")]
    [Route("pause/{id}")]
    [ApeVoloAuthorize(new[] { "admin" })]
    public async Task<ActionResult<object>> Pause(long id)
    {
        var quartzNet = await _quartzNetService.QueryFirstAsync(x => x.Id == id);
        if (quartzNet.IsNull())
        {
            return Error("作业不存在，暂停失败！");
        }

        var triggerStatus = await _schedulerCenterService.GetTriggerStatus(_mapper.Map<QuartzNetDto>(quartzNet));
        if (triggerStatus == "正常")
        {
            //检查任务在内存状态
            var isTrue = await _schedulerCenterService.IsExistScheduleJobAsync(quartzNet);
            if (isTrue)
            {
                if (await _schedulerCenterService.PauseJob(quartzNet))
                {
                    return NoContent();
                }

                return Error("暂停失败，请重试！");
            }

            return Error("暂停失败，作业未启动！");
        }

        return Error("暂停失败，作业非正常状态！");
    }

    /// <summary>
    /// 恢复作业
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPut]
    [Description("恢复作业")]
    [Route("resume/{id}")]
    [ApeVoloAuthorize(new[] { "admin" })]
    public async Task<ActionResult<object>> Resume(long id)
    {
        var quartzNet = await _quartzNetService.QueryFirstAsync(x => x.Id == id);
        if (quartzNet.IsNull())
        {
            return Error("作业不存在，暂停失败！");
        }

        var triggerStatus = await _schedulerCenterService.GetTriggerStatus(_mapper.Map<QuartzNetDto>(quartzNet));
        if (triggerStatus == "暂停")
        {
            //检查任务在内存状态
            var isTrue = await _schedulerCenterService.IsExistScheduleJobAsync(quartzNet);
            if (isTrue)
            {
                if (await _schedulerCenterService.ResumeJob(quartzNet))
                {
                    return NoContent();
                }

                return Error("恢复失败，请重试！");
            }

            return Error("恢复失败，作业未启动！");
        }

        return Error("恢复失败，作业非暂停状态！");
    }


    /// <summary>
    /// 作业调度执行日志
    /// </summary>
    /// <param name="quartzNetLogQueryCriteria"></param>
    /// <param name="pagination"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("logs/list/{id}")]
    [Description("作业调度执行日志")]
    [ApeVoloAuthorize(new[] { "admin" })]
    public async Task<ActionResult<object>> QueryLog(QuartzNetLogQueryCriteria quartzNetLogQueryCriteria,
        Pagination pagination)
    {
        var quartzNetLogList = await _quartzNetLogService.QueryAsync(quartzNetLogQueryCriteria, pagination);

        return new ActionResultVm<QuartzNetLogDto>
        {
            Content = quartzNetLogList,
            TotalElements = pagination.TotalElements
        }.ToJson();
    }

    /// <summary>
    /// 导出作业日志
    /// </summary>
    /// <param name="quartzNetLogQueryCriteria"></param>
    /// <returns></returns>
    [HttpGet]
    [Description("导出作业调度")]
    [Route("logs/download/{id}")]
    //[ApeVoloAuthorize(new string[] { "admin" })]
    public async Task<ActionResult<object>> Download(QuartzNetLogQueryCriteria quartzNetLogQueryCriteria)
    {
        var exportRowModels = await _quartzNetLogService.DownloadAsync(quartzNetLogQueryCriteria);

        var filepath = ExcelHelper.ExportData(exportRowModels, "作业日志列表");

        var provider = new FileExtensionContentTypeProvider();
        FileInfo fileInfo = new FileInfo(filepath);
        var ext = fileInfo.Extension;
        new FileExtensionContentTypeProvider().Mappings.TryGetValue(ext, out var contently);
        return File(await System.IO.File.ReadAllBytesAsync(filepath), contently ?? "application/octet-stream",
            fileInfo.Name);
    }

    #endregion
}