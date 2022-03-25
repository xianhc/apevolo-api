using ApeVolo.Entity.Do.Core;
using ApeVolo.IRepository.Core;
using ApeVolo.IRepository.UnitOfWork;
using ApeVolo.Repository.Base;

namespace ApeVolo.Repository.Core;

public class RoleDeptRepository : SugarHandler<RolesDepartments>, IRoleDeptRepository
{
    public RoleDeptRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }
}