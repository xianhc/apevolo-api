using System.Collections.Generic;
using System.Threading.Tasks;
using Ape.Volo.Common.Model;
using Ape.Volo.Entity.Test;
using Ape.Volo.IBusiness.Base;

namespace Ape.Volo.IBusiness.Interface.Test;

public interface ITestApeVoloService : IBaseServices<TestApeVolo>
{
    #region 基础接口

    Task<bool> CreateAsync(TestApeVolo testApeVolo);
    Task<List<TestApeVolo>> QueryAsync(Pagination pagination);

    #endregion
}
