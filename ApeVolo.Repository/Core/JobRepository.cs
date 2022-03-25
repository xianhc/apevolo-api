using ApeVolo.Entity.Do.Core;
using ApeVolo.IRepository.Core;
using ApeVolo.IRepository.UnitOfWork;
using ApeVolo.Repository.Base;

namespace ApeVolo.Repository.Core;

public class JobRepository : SugarHandler<Job>, IJobRepository
{
    public JobRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }
}