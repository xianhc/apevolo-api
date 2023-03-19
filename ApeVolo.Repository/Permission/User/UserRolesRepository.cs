using ApeVolo.Entity.Permission.User;
using ApeVolo.IRepository.Permission.User;
using ApeVolo.IRepository.UnitOfWork;
using ApeVolo.Repository.Base;

namespace ApeVolo.Repository.Permission.User;

public class UserRolesRepository : SugarHandler<UserRoles>, IUserRolesRepository
{
    public UserRolesRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }
}