using ApeVolo.Common.Model;
using ApeVolo.IBusiness.Base;
using ApeVolo.IBusiness.Dto.Logs;
using ApeVolo.IBusiness.QueryModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApeVolo.Entity.Do.Logs;

namespace ApeVolo.IBusiness.Interface.Logs
{
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
}