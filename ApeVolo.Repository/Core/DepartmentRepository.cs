using ApeVolo.Entity.Do.Core;
using ApeVolo.IRepository.Core;
using ApeVolo.IRepository.UnitOfWork;
using ApeVolo.Repository.Base;

namespace ApeVolo.Repository.Core;

public class DepartmentRepository : SugarHandler<Department>, IDepartmentRepository
{
    public DepartmentRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }
}