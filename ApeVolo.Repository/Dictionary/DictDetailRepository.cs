using ApeVolo.Entity.Do.Dictionary;
using ApeVolo.IRepository.Dictionary;
using ApeVolo.IRepository.UnitOfWork;
using ApeVolo.Repository.Base;

namespace ApeVolo.Repository.Dictionary
{
    public class DictDetailRepository : SugarHandler<DictDetail>, IDictDetailRepository
    {
        public DictDetailRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}