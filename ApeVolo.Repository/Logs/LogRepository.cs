using ApeVolo.Entity.Do.Logs;
using ApeVolo.IRepository.Logs;
using ApeVolo.IRepository.UnitOfWork;
using ApeVolo.Repository.Base;

namespace ApeVolo.Repository.Logs
{
    public class LogRepository : SugarHandler<Log>, ILogRepository
    {
        public LogRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
