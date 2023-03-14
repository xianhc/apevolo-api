using System.Collections.Generic;
using System.Threading.Tasks;
using ApeVolo.Common.Model;
using ApeVolo.Entity.Do.Logs;
using ApeVolo.IBusiness.Base;
using ApeVolo.IBusiness.Dto.Monitor.Logs.Auditing;
using ApeVolo.IBusiness.QueryModel;

namespace ApeVolo.IBusiness.Interface.Monitor.Auditing;

/// <summary>
/// 审计日志接口
/// </summary>
public interface IAuditLogService : IBaseServices<AuditLog>
{
    #region 基础接口

    Task<bool> CreateAsync(AuditLog auditInfo);

    Task<List<AuditLogDto>> QueryAsync(LogQueryCriteria logQueryCriteria, Pagination pagination);

    Task<List<AuditLogDto>> QueryByCurrentAsync(string userName, Pagination pagination);

    #endregion
}