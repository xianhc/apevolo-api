using ApeVolo.Entity.Do.Logs;
using ApeVolo.IRepository.Logs;
using ApeVolo.IRepository.UnitOfWork;
using ApeVolo.Repository.Base;

namespace ApeVolo.Repository.Logs;

public class ExceptionLogRepository : SugarHandler<ExceptionLog>, IExceptionLogRepository
{
    public ExceptionLogRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }
}