using ApeVolo.Entity.System.Dictionary;
using ApeVolo.IRepository.System.Dictionary;
using ApeVolo.IRepository.UnitOfWork;
using ApeVolo.Repository.Base;

namespace ApeVolo.Repository.System.Dictionary;

public class DictRepository : SugarHandler<Dict>, IDictRepository
{
    public DictRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }
}