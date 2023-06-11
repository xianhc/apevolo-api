using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ApeVolo.Business.Base;
using ApeVolo.Common.AttributeExt;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Global;
using ApeVolo.Common.Model;
using ApeVolo.Common.WebApp;
using ApeVolo.Entity.System.Task;
using ApeVolo.IBusiness.Dto.System.Task;
using ApeVolo.IBusiness.ExportModel.System;
using ApeVolo.IBusiness.Interface.System.Task;
using ApeVolo.IBusiness.QueryModel;
using ApeVolo.IRepository.System.Task;
using AutoMapper;

namespace ApeVolo.Business.System.Task;

/// <summary>
/// QuartzNet作业服务
/// </summary>
public class QuartzNetService : BaseServices<QuartzNet>, IQuartzNetService
{
    #region 字段

    private readonly IQuartzNetLogService _quartzNetLogService;

    #endregion

    #region 构造函数

    public QuartzNetService(IQuartzNetRepository taskQuartzRepository, IMapper mapper,
        IQuartzNetLogService quartzNetLogService, ICurrentUser currentUser)
    {
        BaseDal = taskQuartzRepository;
        Mapper = mapper;
        CurrentUser = currentUser;
        _quartzNetLogService = quartzNetLogService;
    }

    #endregion

    #region 基础方法

    public async Task<List<QuartzNet>> QueryAllAsync()
    {
        return await BaseDal.QueryListAsync();
    }


    public async Task<QuartzNet> CreateAsync(CreateUpdateQuartzNetDto createUpdateQuartzNetDto)
    {
        var quartzNet = Mapper.Map<QuartzNet>(createUpdateQuartzNetDto);
        quartzNet.InitEntity(CurrentUser);
        return await BaseDal.AddReturnEntityAsync(quartzNet);
    }

    public async Task<bool> UpdateAsync(CreateUpdateQuartzNetDto createUpdateQuartzNetDto)
    {
        var quartzNet = Mapper.Map<QuartzNet>(createUpdateQuartzNetDto);
        return await UpdateEntityAsync(quartzNet);
    }


    [UseTran]
    public async Task<bool> UpdateJobInfoAsync(QuartzNet quartzNet, QuartzNetLog quartzNetLog)
    {
        await BaseDal.UpdateAsync(quartzNet);
        await _quartzNetLogService.CreateAsync(quartzNetLog);
        return true;
    }

    public async Task<bool> DeleteAsync(List<QuartzNet> quartzNets)
    {
        return await DeleteEntityListAsync(quartzNets);
    }

    public async Task<List<QuartzNetDto>> QueryAsync(QuartzNetQueryCriteria quartzNetQueryCriteria,
        Pagination pagination)
    {
        Expression<Func<QuartzNet, bool>> whereExpression = x => true;
        if (!quartzNetQueryCriteria.TaskName.IsNullOrEmpty())
        {
            whereExpression = whereExpression.AndAlso(x => x.TaskName.Contains(quartzNetQueryCriteria.TaskName));
        }

        if (!quartzNetQueryCriteria.CreateTime.IsNullOrEmpty() && quartzNetQueryCriteria.CreateTime.Count > 1)
        {
            whereExpression = whereExpression.AndAlso(x =>
                x.CreateTime >= quartzNetQueryCriteria.CreateTime[0] &&
                x.CreateTime <= quartzNetQueryCriteria.CreateTime[1]);
        }

        return Mapper.Map<List<QuartzNetDto>>(await BaseDal.QueryPageListAsync(whereExpression, pagination));
    }

    public async Task<List<ExportBase>> DownloadAsync(QuartzNetQueryCriteria quartzNetQueryCriteria)
    {
        var quartzNets = await QueryAsync(quartzNetQueryCriteria, new Pagination { PageSize = 9999 });
        List<ExportBase> quartzExports = new List<ExportBase>();
        quartzExports.AddRange(quartzNets.Select(x => new QuartzNetExport()
        {
            TaskName = x.TaskName,
            TaskGroup = x.TaskGroup,
            Cron = x.Cron,
            AssemblyName = x.AssemblyName,
            ClassName = x.ClassName,
            Description = x.Description,
            Principal = x.Principal,
            AlertEmail = x.AlertEmail,
            PauseAfterFailure = x.PauseAfterFailure,
            RunTimes = x.RunTimes,
            StartTime = x.StartTime,
            EndTime = x.EndTime,
            TriggerType = x.TriggerType,
            IntervalSecond = x.IntervalSecond,
            CycleRunTimes = x.CycleRunTimes,
            IsEnable = x.IsEnable ? BoolState.True : BoolState.False,
            RunParams = x.RunParams,
            TriggerStatus = x.TriggerStatus,
            TriggerTypeStr = x.TriggerType == 1 ? "cron" : "simple",
            CreateTime = x.CreateTime
        }));
        return quartzExports;
    }

    #endregion
}