using ApeVolo.Entity.Do.Logs;
using ApeVolo.IRepository.Monitor.Exception;
using ApeVolo.IRepository.UnitOfWork;
using ApeVolo.Repository.Base;

namespace ApeVolo.Repository.Monitor.Exception;

public class ExceptionLogRepository : SugarHandler<ExceptionLog>, IExceptionLogRepository
{
    public ExceptionLogRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }
}