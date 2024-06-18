using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Ape.Volo.Business.Base;
using Ape.Volo.Common.Extensions;
using Ape.Volo.Common.Model;
using Ape.Volo.Common.WebApp;
using Ape.Volo.Entity.Monitor;
using Ape.Volo.IBusiness.Dto.Monitor;
using Ape.Volo.IBusiness.Interface.Monitor;
using Ape.Volo.IBusiness.QueryModel;

namespace Ape.Volo.Business.Monitor;

/// <summary>
/// 系统日志服务
/// </summary>
public class ExceptionLogService : BaseServices<ExceptionLog>, IExceptionLogService
{
    #region 构造函数

    public ExceptionLogService(ApeContext apeContext) : base(apeContext)
    {
    }

    #endregion

    #region 基础方法

    public async Task<bool> CreateAsync(ExceptionLog exceptionLog)
    {
        //return await AddEntityAsync(exceptionLog);
        return await SugarRepository.SugarClient.Insertable(exceptionLog).SplitTable().ExecuteCommandAsync() > 0;
    }

    public async Task<List<ExceptionLogDto>> QueryAsync(LogQueryCriteria logQueryCriteria, Pagination pagination)
    {
        Expression<Func<ExceptionLog, bool>> whereLambda = l => true;
        if (!logQueryCriteria.KeyWords.IsNullOrEmpty())
        {
            whereLambda = whereLambda.AndAlso(l => l.Description.Contains(logQueryCriteria.KeyWords));
        }

        if (!logQueryCriteria.CreateTime.IsNullOrEmpty())
        {
            whereLambda = whereLambda.AndAlso(l =>
                l.CreateTime >= logQueryCriteria.CreateTime[0] && l.CreateTime <= logQueryCriteria.CreateTime[1]);
        }

        var queryOptions = new QueryOptions<ExceptionLog>
        {
            Pagination = pagination,
            WhereLambda = whereLambda,
            IsSplitTable = true
        };
        var logs = await SugarRepository.QueryPageListAsync(queryOptions);
        return ApeContext.Mapper.Map<List<ExceptionLogDto>>(logs);
    }

    #endregion
}
