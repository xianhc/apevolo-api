using ApeVolo.Entity.Do.Email;
using ApeVolo.IRepository.Email;
using ApeVolo.IRepository.UnitOfWork;
using ApeVolo.Repository.Base;

namespace ApeVolo.Repository.Email;

public class EmailMessageTemplateRepository : SugarHandler<EmailMessageTemplate>, IEmailMessageTemplateRepository
{
    public EmailMessageTemplateRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }
}