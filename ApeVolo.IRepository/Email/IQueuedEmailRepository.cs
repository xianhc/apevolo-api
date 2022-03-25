using ApeVolo.Entity.Do.Email;
using ApeVolo.IRepository.Base;

namespace ApeVolo.IRepository.Email;

public interface IQueuedEmailRepository : ISugarHandler<QueuedEmail>
{
}