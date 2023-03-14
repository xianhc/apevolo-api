using ApeVolo.IRepository.Permission.Menu;
using ApeVolo.IRepository.UnitOfWork;
using ApeVolo.Repository.Base;

namespace ApeVolo.Repository.Permission.Menu;

public class MenuRepository : SugarHandler<Entity.Do.Core.Menu>, IMenuRepository
{
    public MenuRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }
}