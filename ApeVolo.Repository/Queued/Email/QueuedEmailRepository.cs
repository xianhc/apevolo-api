using ApeVolo.Entity.Queued;
using ApeVolo.IRepository.Queued.Email;
using ApeVolo.IRepository.UnitOfWork;
using ApeVolo.Repository.Base;

namespace ApeVolo.Repository.Queued.Email;

public class QueuedEmailRepository : SugarHandler<QueuedEmail>, IQueuedEmailRepository
{
    public QueuedEmailRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }
}