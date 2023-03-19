using ApeVolo.IRepository.System.Setting;
using ApeVolo.IRepository.UnitOfWork;
using ApeVolo.Repository.Base;

namespace ApeVolo.Repository.System.Setting;

public class SettingRepository : SugarHandler<Entity.System.Setting>, ISettingRepository
{
    public SettingRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }
}