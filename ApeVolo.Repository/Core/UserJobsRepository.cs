using ApeVolo.Entity.Do.Core;
using ApeVolo.IRepository.Core;
using ApeVolo.IRepository.UnitOfWork;
using ApeVolo.Repository.Base;

namespace ApeVolo.Repository.Core
{
    public class UserJobsRepository : SugarHandler<UserJobs>, IUserJobsRepository
    {
        public UserJobsRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
