using ApeVolo.Entity.Permission.User;
using ApeVolo.IRepository.Permission.User;
using ApeVolo.IRepository.UnitOfWork;
using ApeVolo.Repository.Base;

namespace ApeVolo.Repository.Permission.User;

public class UserJobsRepository : SugarHandler<UserJobs>, IUserJobsRepository
{
    public UserJobsRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }
}