using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ApeVolo.Business.Base;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Model;
using ApeVolo.Entity.Do.Logs;
using ApeVolo.IBusiness.Dto.Logs;
using ApeVolo.IBusiness.Interface.Logs;
using ApeVolo.IBusiness.QueryModel;
using ApeVolo.IRepository.Logs;
using AutoMapper;

namespace ApeVolo.Business.Impl.Logs;

/// <summary>
/// 系统日志服务
/// </summary>
public class LogService : BaseServices<Log>, ILogService
{
    #region 构造函数

    public LogService(ILogRepository logRepository, IMapper mapper)
    {
        _baseDal = logRepository;
        _mapper = mapper;
    }

    #endregion

    #region 基础方法

    public async Task<bool> CreateAsync(Log log)
    {
        return await AddEntityAsync(log);
    }


    public async Task<List<LogDto>> QueryAsync(LogQueryCriteria logQueryCriteria, Pagination pagination)
    {
        Expression<Func<Log, bool>> whereLambda = l => l.IsDeleted == false;
        if (!logQueryCriteria.KeyWords.IsNullOrEmpty())
        {
            whereLambda = whereLambda.And(l => l.Description.Contains(logQueryCriteria.KeyWords));
        }

        if (!logQueryCriteria.CreateTime.IsNullOrEmpty())
        {
            whereLambda = whereLambda.And(l =>
                l.CreateTime >= logQueryCriteria.CreateTime[0] && l.CreateTime <= logQueryCriteria.CreateTime[1]);
        }

        var logs = await _baseDal.QueryPageListAsync(whereLambda, pagination);
        return _mapper.Map<List<LogDto>>(logs);
    }

    #endregion
}