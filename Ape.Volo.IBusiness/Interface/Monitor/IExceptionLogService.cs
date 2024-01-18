using System.Collections.Generic;
using System.Threading.Tasks;
using Ape.Volo.Common.Model;
using Ape.Volo.IBusiness.Base;
using Ape.Volo.IBusiness.Dto.Monitor;
using Ape.Volo.IBusiness.QueryModel;
using ApeVolo.Entity.Monitor;

namespace Ape.Volo.IBusiness.Interface.Monitor;

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
