using ApeVolo.Entity.Do.Core;
using ApeVolo.IRepository.Core;
using ApeVolo.IRepository.UnitOfWork;
using ApeVolo.Repository.Base;

namespace ApeVolo.Repository.Core
{
    public class RolesMenusRepository : SugarHandler<RoleMenu>, IRolesMenusRepository
    {
        public RolesMenusRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}