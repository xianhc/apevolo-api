using ApeVolo.Entity.Do.Core;
using ApeVolo.IRepository.Test;
using ApeVolo.IRepository.UnitOfWork;
using ApeVolo.Repository.Base;

namespace ApeVolo.Repository.Test;

public class TestApeVoloRepostiory : SugarHandler<TestApeVolo>, ITestApeVoloRepostiory
{
    public TestApeVoloRepostiory(IUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }
}