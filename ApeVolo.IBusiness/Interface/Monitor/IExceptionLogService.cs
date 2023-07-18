using System.Collections.Generic;
using System.Threading.Tasks;
using ApeVolo.Common.Model;
using ApeVolo.Entity.Monitor;
using ApeVolo.IBusiness.Base;
using ApeVolo.IBusiness.Dto.Monitor;
using ApeVolo.IBusiness.QueryModel;

namespace ApeVolo.IBusiness.Interface.Monitor;

/// <summary>
/// 系统日志接口
/// </summary>
public interface IExceptionLogService : IBaseServices<ExceptionLog>
{
    #region 基础接口

    Task<bool> CreateAsync(ExceptionLog exceptionLog);

    Task<List<ExceptionLogDto>> QueryAsync(LogQueryCriteria logQueryCriteria, Pagination pagination);

    #endregion
}
