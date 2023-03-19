using ApeVolo.Entity.Message.Email;
using ApeVolo.IRepository.Message.Email.Template;
using ApeVolo.IRepository.UnitOfWork;
using ApeVolo.Repository.Base;

namespace ApeVolo.Repository.Message.Email.Template;

public class EmailMessageTemplateRepository : SugarHandler<EmailMessageTemplate>, IEmailMessageTemplateRepository
{
    public EmailMessageTemplateRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }
}