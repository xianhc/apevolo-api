using ApeVolo.IRepository.Permission.Department;
using ApeVolo.IRepository.UnitOfWork;
using ApeVolo.Repository.Base;

namespace ApeVolo.Repository.Permission.Department;

public class DepartmentRepository : SugarHandler<Entity.Do.Core.Department>, IDepartmentRepository
{
    public DepartmentRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }
}