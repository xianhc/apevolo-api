using ApeVolo.IRepository.Permission.User;
using ApeVolo.IRepository.UnitOfWork;
using ApeVolo.Repository.Base;

namespace ApeVolo.Repository.Permission.User;

public class UserRepository : SugarHandler<Entity.Do.Core.User>, IUserRepository
{
    public UserRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }
}