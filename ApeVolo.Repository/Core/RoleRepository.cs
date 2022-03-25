using ApeVolo.Entity.Do.Core;
using ApeVolo.IRepository.Core;
using ApeVolo.IRepository.UnitOfWork;
using ApeVolo.Repository.Base;

namespace ApeVolo.Repository.Core;

public class RoleRepository : SugarHandler<Role>, IRoleRepository
{
    public RoleRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }
}