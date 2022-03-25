using ApeVolo.Entity.Do.Tasks;
using ApeVolo.IRepository.Tasks;
using ApeVolo.IRepository.UnitOfWork;
using ApeVolo.Repository.Base;

namespace ApeVolo.Repository.Tasks;

public class QuartzNetRepository : SugarHandler<QuartzNet>, IQuartzNetRepository
{
    public QuartzNetRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }
}