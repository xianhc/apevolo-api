using ApeVolo.Entity.Monitor.Logs;
using ApeVolo.IRepository.Base;

namespace ApeVolo.IRepository.Monitor.Auditing;

public interface IAuditLogRepository : ISugarHandler<AuditLog>
{
}