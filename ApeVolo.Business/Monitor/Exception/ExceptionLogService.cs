using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ApeVolo.Business.Base;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Model;
using ApeVolo.Entity.Do.Logs;
using ApeVolo.IBusiness.Dto.Monitor.Logs.Exception;
using ApeVolo.IBusiness.Interface.Monitor.Exception;
using ApeVolo.IBusiness.QueryModel;
using ApeVolo.IRepository.Monitor.Exception;
using AutoMapper;

namespace ApeVolo.Business.Monitor.Exception;

/// <summary>
/// 系统日志服务
/// </summary>
public class ExceptionLogService : BaseServices<ExceptionLog>, IExceptionLogService
{
    #region 构造函数

    public ExceptionLogService(IExceptionLogRepository exceptionLogRepository, IMapper mapper)
    {
        BaseDal = exceptionLogRepository;
        Mapper = mapper;
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

        var logs = await BaseDal.QueryPageListAsync(whereLambda, pagination);
        return Mapper.Map<List<ExceptionLogDto>>(logs);
    }

    #endregion
}