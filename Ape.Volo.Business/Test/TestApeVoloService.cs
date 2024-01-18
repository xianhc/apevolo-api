using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Ape.Volo.Business.Base;
using Ape.Volo.Common.Model;
using Ape.Volo.IBusiness.Interface.Test;
using ApeVolo.Entity.Test;

namespace Ape.Volo.Business.Test;

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
