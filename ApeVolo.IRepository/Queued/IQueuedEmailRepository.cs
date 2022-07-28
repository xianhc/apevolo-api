using ApeVolo.Entity.Do.Queued;
using ApeVolo.IRepository.Base;

namespace ApeVolo.IRepository.Queued;

public interface IQueuedEmailRepository : ISugarHandler<QueuedEmail>
{
}