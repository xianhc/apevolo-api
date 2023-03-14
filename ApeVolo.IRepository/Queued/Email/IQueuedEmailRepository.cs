using ApeVolo.Entity.Do.Queued;
using ApeVolo.IRepository.Base;

namespace ApeVolo.IRepository.Queued.Email;

public interface IQueuedEmailRepository : ISugarHandler<QueuedEmail>
{
}