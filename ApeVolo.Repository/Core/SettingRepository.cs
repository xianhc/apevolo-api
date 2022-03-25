using ApeVolo.Entity.Do.Core;
using ApeVolo.IRepository.Core;
using ApeVolo.IRepository.UnitOfWork;
using ApeVolo.Repository.Base;

namespace ApeVolo.Repository.Core;

public class SettingRepository : SugarHandler<Setting>, ISettingRepository
{
    public SettingRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }
}