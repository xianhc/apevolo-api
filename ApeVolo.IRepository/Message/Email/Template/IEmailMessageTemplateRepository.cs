using ApeVolo.Entity.Message.Email;
using ApeVolo.IRepository.Base;

namespace ApeVolo.IRepository.Message.Email.Template;

public interface IEmailMessageTemplateRepository : ISugarHandler<EmailMessageTemplate>
{
}