using System.Collections.Generic;
using System.Threading.Tasks;
using ApeVolo.Common.Model;
using ApeVolo.Entity.Do.Logs;
using ApeVolo.IBusiness.Base;
using ApeVolo.IBusiness.Dto.Logs;
using ApeVolo.IBusiness.QueryModel;

namespace ApeVolo.IBusiness.Interface.Logs;

/// <summary>
/// 系统日志接口
/// </summary>
public interface ILogService : IBaseServices<Log>
{
    #region 基础接口

    Task<bool> CreateAsync(Log log);

    Task<List<LogDto>> QueryAsync(LogQueryCriteria logQueryCriteria, Pagination pagination);

    #endregion
}