using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ApeVolo.Business.Base;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Global;
using ApeVolo.Common.Model;
using ApeVolo.Entity.System.Task;
using ApeVolo.IBusiness.Dto.System.Task;
using ApeVolo.IBusiness.ExportModel.System;
using ApeVolo.IBusiness.Interface.System.Task;
using ApeVolo.IBusiness.QueryModel;
using ApeVolo.IRepository.System.Task;
using AutoMapper;

namespace ApeVolo.Business.System.Task;

/// <summary>
/// QuartzNet作业日志服务
/// </summary>
public class QuartzNetLogService : BaseServices<QuartzNetLog>, IQuartzNetLogService
{
    #region 构造函数

    public QuartzNetLogService(IQuartzNetLogRepository taskQuartzLogRepository, IMapper mapper)
    {
        BaseDal = taskQuartzLogRepository;
        Mapper = mapper;
    }

    #endregion

    #region 基础方法

    public async Task<bool> CreateAsync(QuartzNetLog quartzNetLog)
    {
        return await BaseDal.AddReturnBoolAsync(quartzNetLog);
    }

    public async Task<List<QuartzNetLogDto>> QueryAsync(QuartzNetLogQueryCriteria quartzNetLogQueryCriteria,
        Pagination pagination)
    {
        Expression<Func<QuartzNetLog, bool>> whereExpression = x => true;

        if (!quartzNetLogQueryCriteria.Id.IsNullOrEmpty())
        {
            whereExpression = whereExpression.AndAlso(x => x.TaskId == quartzNetLogQueryCriteria.Id);
        }

        if (quartzNetLogQueryCriteria.IsSuccess.HasValue)
        {
            whereExpression = whereExpression.AndAlso(x => x.IsSuccess == quartzNetLogQueryCriteria.IsSuccess);
        }

        if (!quartzNetLogQueryCriteria.CreateTime.IsNullOrEmpty() && quartzNetLogQueryCriteria.CreateTime.Count > 1)
        {
            whereExpression = whereExpression.AndAlso(x =>
                x.CreateTime >= quartzNetLogQueryCriteria.CreateTime[0] &&
                x.CreateTime <= quartzNetLogQueryCriteria.CreateTime[1]);
        }

        return Mapper.Map<List<QuartzNetLogDto>>(await BaseDal.QueryPageListAsync(whereExpression, pagination));
    }

    public async Task<List<ExportBase>> DownloadAsync(QuartzNetLogQueryCriteria quartzNetLogQueryCriteria)
    {
        var quartzNetLogs = await QueryAsync(quartzNetLogQueryCriteria, new Pagination { PageSize = 9999 });
        List<ExportBase> quartzNetLogExports = new List<ExportBase>();
        quartzNetLogExports.AddRange(quartzNetLogs.Select(x => new QuartzNetLogExport()
        {
            TaskId = x.TaskId,
            TaskName = x.TaskName,
            TaskGroup = x.TaskGroup,
            AssemblyName = x.AssemblyName,
            ClassName = x.ClassName,
            Cron = x.Cron,
            ExceptionDetail = x.ExceptionDetail,
            ExecutionDuration = x.ExecutionDuration,
            RunParams = x.RunParams,
            IsSuccess = x.IsSuccess ? BoolState.True : BoolState.False,
            CreateTime = x.CreateTime
        }));
        return quartzNetLogExports;
    }

    #endregion
}