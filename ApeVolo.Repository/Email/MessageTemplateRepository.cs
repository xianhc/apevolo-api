using ApeVolo.Entity.Do.Email;
using ApeVolo.IRepository.Email;
using ApeVolo.IRepository.UnitOfWork;
using ApeVolo.Repository.Base;

namespace ApeVolo.Repository.Email;

public class MessageTemplateRepository : SugarHandler<MessageTemplate>, IMessageTemplateRepository
{
    public MessageTemplateRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }
}