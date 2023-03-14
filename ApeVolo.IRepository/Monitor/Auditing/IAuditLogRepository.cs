using ApeVolo.Entity.Do.Logs;
using ApeVolo.IRepository.Base;

namespace ApeVolo.IRepository.Monitor.Auditing;

public interface IAuditLogRepository : ISugarHandler<AuditLog>
{
}