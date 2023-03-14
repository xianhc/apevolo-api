using ApeVolo.IRepository.System.AppSecret;
using ApeVolo.IRepository.UnitOfWork;
using ApeVolo.Repository.Base;

namespace ApeVolo.Repository.System.AppSecret;

public class AppSecretRepository : SugarHandler<Entity.Do.Core.AppSecret>, IAppSecretRepository
{
    public AppSecretRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }
}