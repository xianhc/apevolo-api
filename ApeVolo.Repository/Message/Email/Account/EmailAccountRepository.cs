using ApeVolo.Entity.Message.Email;
using ApeVolo.IRepository.Message.Email.Account;
using ApeVolo.IRepository.UnitOfWork;
using ApeVolo.Repository.Base;

namespace ApeVolo.Repository.Message.Email.Account;

public class EmailAccountRepository : SugarHandler<EmailAccount>, IEmailAccountRepository
{
    public EmailAccountRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }
}