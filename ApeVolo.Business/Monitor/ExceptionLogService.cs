using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ApeVolo.Business.Base;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Model;
using ApeVolo.Common.WebApp;
using ApeVolo.Entity.Monitor;
using ApeVolo.IBusiness.Dto.Monitor;
using ApeVolo.IBusiness.Interface.Monitor;
using ApeVolo.IBusiness.QueryModel;

namespace ApeVolo.Business.Monitor;

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
        return await AddEntityAsync(exceptionLog);
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

        var logs = await SugarRepository.QueryPageListAsync(whereLambda, pagination);
        return ApeContext.Mapper.Map<List<ExceptionLogDto>>(logs);
    }

    #endregion
}
