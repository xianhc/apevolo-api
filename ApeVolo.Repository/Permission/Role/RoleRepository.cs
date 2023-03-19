using ApeVolo.IRepository.Permission.Role;
using ApeVolo.IRepository.UnitOfWork;
using ApeVolo.Repository.Base;

namespace ApeVolo.Repository.Permission.Role;

public class RoleRepository : SugarHandler<Entity.Permission.Role.Role>, IRoleRepository
{
    public RoleRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }
}