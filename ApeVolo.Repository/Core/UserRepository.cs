using ApeVolo.Entity.Do.Core;
using ApeVolo.IRepository.Core;
using ApeVolo.IRepository.UnitOfWork;
using ApeVolo.Repository.Base;

namespace ApeVolo.Repository.CoreModule;

public class UserRepository : SugarHandler<User>, IUserRepository
{
    public UserRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }
}