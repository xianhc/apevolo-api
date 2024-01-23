using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Ape.Volo.Business.Base;
using Ape.Volo.Common.Extention;
using Ape.Volo.Common.Global;
using Ape.Volo.Common.Model;
using Ape.Volo.Common.WebApp;
using Ape.Volo.Entity.System;
using Ape.Volo.IBusiness.Dto.System;
using Ape.Volo.IBusiness.ExportModel.System;
using Ape.Volo.IBusiness.Interface.System;
using Ape.Volo.IBusiness.QueryModel;

namespace Ape.Volo.Business.System;

/// <summary>
/// QuartzNet作业日志服务
/// </summary>
public class QuartzNetLogService : BaseServices<QuartzNetLog>, IQuartzNetLogService
{
    #region 构造函数

    public QuartzNetLogService(ApeContext apeContext) : base(apeContext)
    {
    }

    #endregion

    #region 基础方法

    public async Task<bool> CreateAsync(QuartzNetLog quartzNetLog)
    {
        return await SugarRepository.AddReturnBoolAsync(quartzNetLog);
    }

    public async Task<List<QuartzNetLogDto>> QueryAsync(QuartzNetLogQueryCriteria quartzNetLogQueryCriteria,
        Pagination pagination)
    {
        var whereExpression = GetWhereExpression(quartzNetLogQueryCriteria);
        return ApeContext.Mapper.Map<List<QuartzNetLogDto>>(
            await SugarRepository.QueryPageListAsync(whereExpression, pagination));
    }

    public async Task<List<ExportBase>> DownloadAsync(QuartzNetLogQueryCriteria quartzNetLogQueryCriteria)
    {
        var whereExpression = GetWhereExpression(quartzNetLogQueryCriteria);
        var quartzNetLogs = await TableWhere(whereExpression).ToListAsync();
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


    #region 条件表达式

    private static Expression<Func<QuartzNetLog, bool>> GetWhereExpression(
        QuartzNetLogQueryCriteria quartzNetLogQueryCriteria)
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

        return whereExpression;
    }

    #endregion
}
