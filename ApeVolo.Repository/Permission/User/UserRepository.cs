using ApeVolo.IRepository.Permission.User;
using ApeVolo.IRepository.UnitOfWork;
using ApeVolo.Repository.Base;

namespace ApeVolo.Repository.Permission.User;

public class UserRepository : SugarHandler<Entity.Permission.User.User>, IUserRepository
{
    public UserRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }
}