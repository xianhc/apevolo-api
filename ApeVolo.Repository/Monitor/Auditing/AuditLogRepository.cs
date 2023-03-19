using ApeVolo.Entity.Monitor.Logs;
using ApeVolo.IRepository.Monitor.Auditing;
using ApeVolo.IRepository.UnitOfWork;
using ApeVolo.Repository.Base;

namespace ApeVolo.Repository.Monitor.Auditing;

public class AuditLogRepository : SugarHandler<AuditLog>, IAuditLogRepository
{
    public AuditLogRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }
}