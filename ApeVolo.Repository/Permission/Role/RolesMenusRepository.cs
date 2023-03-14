using ApeVolo.Entity.Do.Core;
using ApeVolo.IRepository.Permission.Role;
using ApeVolo.IRepository.UnitOfWork;
using ApeVolo.Repository.Base;

namespace ApeVolo.Repository.Permission.Role;

public class RolesMenusRepository : SugarHandler<RoleMenu>, IRolesMenusRepository
{
    public RolesMenusRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }
}