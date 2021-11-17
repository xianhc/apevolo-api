using ApeVolo.Entity.Do.Core;
using ApeVolo.IRepository.Core;
using ApeVolo.IRepository.UnitOfWork;
using ApeVolo.Repository.Base;

namespace ApeVolo.Repository.Core
{
    public class AppSecretRepository : SugarHandler<AppSecret>, IAppSecretRepository
    {
        public AppSecretRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}