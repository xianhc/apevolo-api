using ApeVolo.Entity.Do.Dictionary;
using ApeVolo.IRepository.Dictionary;
using ApeVolo.IRepository.UnitOfWork;
using ApeVolo.Repository.Base;

namespace ApeVolo.Repository.Dictionary;

public class DictRepository : SugarHandler<Dict>, IDictRepository
{
    public DictRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }
}