using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ApeVolo.Business.Base;
using ApeVolo.Common.Model;
using ApeVolo.Entity.Do.Core;
using ApeVolo.IBusiness.Interface.Core;
using ApeVolo.IRepository.Core;

namespace ApeVolo.Business.Impl.Core;

public class TestApeVoloService : BaseServices<TestApeVolo>, ITestApeVoloService
{
    public TestApeVoloService(ITestApeVoloRepostiory testApeVoloRepostiory)
    {
        BaseDal = testApeVoloRepostiory;
    }

    public async Task<bool> CreateAsync(TestApeVolo testApeVolo)
    {
        return await AddEntityAsync(testApeVolo);
    }

    public async Task<List<TestApeVolo>> QueryAsync(Pagination pagination)
    {
        Expression<Func<TestApeVolo, bool>> whereExpression = x => x.IsDeleted == false;
        return Mapper.Map<List<TestApeVolo>>(await BaseDal.QueryPageListAsync(whereExpression, pagination));
    }
}