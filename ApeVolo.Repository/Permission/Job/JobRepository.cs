using ApeVolo.IRepository.Permission.Job;
using ApeVolo.IRepository.UnitOfWork;
using ApeVolo.Repository.Base;

namespace ApeVolo.Repository.Permission.Job;

public class JobRepository : SugarHandler<Entity.Do.Core.Job>, IJobRepository
{
    public JobRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }
}