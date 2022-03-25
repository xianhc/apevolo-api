using ApeVolo.Entity.Do.Core;
using ApeVolo.IRepository.Core;
using ApeVolo.IRepository.UnitOfWork;
using ApeVolo.Repository.Base;

namespace ApeVolo.Repository.Core;

public class UserRolesRepository : SugarHandler<UserRoles>, IUserRolesRepository
{
    public UserRolesRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }
}