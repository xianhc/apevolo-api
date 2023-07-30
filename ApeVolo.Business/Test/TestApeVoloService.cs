using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ApeVolo.Business.Base;
using ApeVolo.Common.Model;
using ApeVolo.Entity.Test;
using ApeVolo.IBusiness.Interface.Test;

namespace ApeVolo.Business.Test;

public class TestApeVoloService : BaseServices<TestApeVolo>, ITestApeVoloService
{
    public async Task<bool> CreateAsync(TestApeVolo testApeVolo)
    {
        return await AddEntityAsync(testApeVolo);
    }

    public async Task<List<TestApeVolo>> QueryAsync(Pagination pagination)
    {
        Expression<Func<TestApeVolo, bool>> whereExpression = x => true;
        return await SugarRepository.QueryPageListAsync(whereExpression, pagination);
    }
}
