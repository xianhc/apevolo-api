using ApeVolo.Entity.Do.Logs;
using ApeVolo.IRepository.Logs;
using ApeVolo.IRepository.UnitOfWork;
using ApeVolo.Repository.Base;

namespace ApeVolo.Repository.Logs
{
    public class AuditLogRepository : SugarHandler<AuditLog>, IAuditLogRepository
    {
        public AuditLogRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
