using System.Collections.Generic;
using System.Threading.Tasks;
using Ape.Volo.Common.Model;
using Ape.Volo.Entity.Monitor;
using Ape.Volo.IBusiness.Base;
using Ape.Volo.IBusiness.Dto.Monitor;
using Ape.Volo.IBusiness.QueryModel;

namespace Ape.Volo.IBusiness.Interface.Monitor;

/// <summary>
/// 审计日志接口
/// </summary>
public interface IAuditLogService : IBaseServices<AuditLog>
{
    #region 基础接口

    /// <summary>
    /// 创建
    /// </summary>
    /// <param name="auditLog"></param>
    /// <returns></returns>
    Task<bool> CreateAsync(AuditLog auditLog);


    /// <summary>
    /// 创建
    /// </summary>
    /// <param name="auditLogs"></param>
    /// <returns></returns>
    Task<bool> CreateListAsync(List<AuditLog> auditLogs);


    /// <summary>
    /// 查询
    /// </summary>
    /// <param name="logQueryCriteria"></param>
    /// <param name="pagination"></param>
    /// <returns></returns>
    Task<List<AuditLogDto>> QueryAsync(LogQueryCriteria logQueryCriteria, Pagination pagination);

    /// <summary>
    /// 查询(个人)
    /// </summary>
    /// <param name="pagination"></param>
    /// <returns></returns>
    Task<List<AuditLogDto>> QueryByCurrentAsync(Pagination pagination);

    #endregion
}
