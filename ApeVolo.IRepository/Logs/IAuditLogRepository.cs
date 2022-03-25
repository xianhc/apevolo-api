using ApeVolo.Entity.Do.Logs;
using ApeVolo.IRepository.Base;

namespace ApeVolo.IRepository.Logs;

public interface IAuditLogRepository : ISugarHandler<AuditLog>
{
}