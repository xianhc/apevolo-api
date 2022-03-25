using ApeVolo.Entity.Do.Tasks;
using ApeVolo.IRepository.Tasks;
using ApeVolo.IRepository.UnitOfWork;
using ApeVolo.Repository.Base;

namespace ApeVolo.Repository.Tasks;

public class QuartzNetLogRepository : SugarHandler<QuartzNetLog>, IQuartzNetLogRepository
{
    public QuartzNetLogRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }
}