using ApeVolo.Entity.Do.Email;
using ApeVolo.IRepository.Email;
using ApeVolo.IRepository.UnitOfWork;
using ApeVolo.Repository.Base;

namespace ApeVolo.Repository.Email;

public class QueuedEmailRepository : SugarHandler<QueuedEmail>, IQueuedEmailRepository
{
    public QueuedEmailRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }
}