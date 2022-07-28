using ApeVolo.Entity.Do.Queued;
using ApeVolo.IRepository.Queued;
using ApeVolo.IRepository.UnitOfWork;
using ApeVolo.Repository.Base;

namespace ApeVolo.Repository.Queued;

public class QueuedEmailRepository : SugarHandler<QueuedEmail>, IQueuedEmailRepository
{
    public QueuedEmailRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }
}