using System.Collections.Generic;
using System.Threading.Tasks;
using ApeVolo.Common.Model;
using ApeVolo.Entity.Do.Core;
using ApeVolo.IBusiness.Base;

namespace ApeVolo.IBusiness.Interface.Test;

public interface ITestApeVoloService : IBaseServices<TestApeVolo>
{
    #region 基础接口

    Task<bool> CreateAsync(TestApeVolo testApeVolo);
    Task<List<TestApeVolo>> QueryAsync(Pagination pagination);

    #endregion
}