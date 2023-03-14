using ApeVolo.Entity.Do.Core;
using ApeVolo.IRepository.Permission.Role;
using ApeVolo.IRepository.UnitOfWork;
using ApeVolo.Repository.Base;

namespace ApeVolo.Repository.Permission.Role;

public class RoleDeptRepository : SugarHandler<RolesDepartments>, IRoleDeptRepository
{
    public RoleDeptRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }
}