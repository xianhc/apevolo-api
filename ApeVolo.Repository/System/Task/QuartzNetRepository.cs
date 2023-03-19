using ApeVolo.Entity.System.Task;
using ApeVolo.IRepository.System.Task;
using ApeVolo.IRepository.UnitOfWork;
using ApeVolo.Repository.Base;

namespace ApeVolo.Repository.System.Task;

public class QuartzNetRepository : SugarHandler<QuartzNet>, IQuartzNetRepository
{
    public QuartzNetRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }
}