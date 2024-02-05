using System.Collections.Generic;
using System.Threading.Tasks;
using Ape.Volo.Common.Model;
using Ape.Volo.Entity.Test;
using Ape.Volo.IBusiness.Base;

namespace Ape.Volo.IBusiness.Interface.Test;

/// <summary>
/// 测试接口
/// </summary>
public interface ITestApeVoloService : IBaseServices<TestApeVolo>
{
    #region 基础接口

    /// <summary>
    /// 创建
    /// </summary>
    /// <param name="testApeVolo"></param>
    /// <returns></returns>
    Task<bool> CreateAsync(TestApeVolo testApeVolo);

    /// <summary>
    /// 查询
    /// </summary>
    /// <param name="pagination"></param>
    /// <returns></returns>
    Task<List<TestApeVolo>> QueryAsync(Pagination pagination);

    #endregion
}
