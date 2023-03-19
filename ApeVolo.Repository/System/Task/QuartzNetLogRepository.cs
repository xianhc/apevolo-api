using ApeVolo.Entity.System.Task;
using ApeVolo.IRepository.System.Task;
using ApeVolo.IRepository.UnitOfWork;
using ApeVolo.Repository.Base;

namespace ApeVolo.Repository.System.Task;

public class QuartzNetLogRepository : SugarHandler<QuartzNetLog>, IQuartzNetLogRepository
{
    public QuartzNetLogRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }
}