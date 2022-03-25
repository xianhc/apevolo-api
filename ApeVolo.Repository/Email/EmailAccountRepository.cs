using ApeVolo.Entity.Do.Email;
using ApeVolo.IRepository.Email;
using ApeVolo.IRepository.UnitOfWork;
using ApeVolo.Repository.Base;

namespace ApeVolo.Repository.Email;

public class EmailAccountRepository : SugarHandler<EmailAccount>, IEmailAccountRepository
{
    public EmailAccountRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }
}