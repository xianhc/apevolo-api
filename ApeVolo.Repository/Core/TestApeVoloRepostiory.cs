using ApeVolo.Entity.Do.Core;
using ApeVolo.IRepository.Core;
using ApeVolo.IRepository.UnitOfWork;
using ApeVolo.Repository.Base;

namespace ApeVolo.Repository.Core
{
    public class TestApeVoloRepostiory : SugarHandler<TestApeVolo>, ITestApeVoloRepostiory
    {
        public TestApeVoloRepostiory(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}