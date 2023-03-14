using ApeVolo.Entity.Do.Dictionary;
using ApeVolo.IRepository.System.Dictionary;
using ApeVolo.IRepository.UnitOfWork;
using ApeVolo.Repository.Base;

namespace ApeVolo.Repository.System.Dictionary;

public class DictDetailRepository : SugarHandler<DictDetail>, IDictDetailRepository
{
    public DictDetailRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }
}