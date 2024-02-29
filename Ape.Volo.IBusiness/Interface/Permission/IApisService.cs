using System.Collections.Generic;
using System.Threading.Tasks;
using Ape.Volo.Common.Model;
using Ape.Volo.Entity.Permission;
using Ape.Volo.IBusiness.Dto.Permission;
using Ape.Volo.IBusiness.QueryModel;

namespace Ape.Volo.IBusiness.Interface.Permission;

/// <summary>
/// apis 接口
/// </summary>
public interface IApisService
{
    /// <summary>
    /// 创建
    /// </summary>
    /// <param name="createUpdateApisDto"></param>
    /// <returns></returns>
    Task<bool> CreateAsync(CreateUpdateApisDto createUpdateApisDto);

    /// <summary>
    /// 更新
    /// </summary>
    /// <param name="createUpdateApisDto"></param>
    /// <returns></returns>
    Task<bool> UpdateAsync(CreateUpdateApisDto createUpdateApisDto);

    /// <summary>
    /// 删除
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    Task<bool> DeleteAsync(HashSet<long> ids);

    /// <summary>
    /// 查询
    /// </summary>
    /// <param name="apisQueryCriteria"></param>
    /// <param name="pagination"></param>
    /// <returns></returns>
    Task<List<Apis>> QueryAsync(ApisQueryCriteria apisQueryCriteria, Pagination pagination);

    /// <summary>
    /// 查询所有
    /// </summary>
    /// <returns></returns>
    Task<List<Apis>> QueryAllAsync();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="apis"></param>
    /// <returns></returns>
    Task<bool> CreateAsync(List<Apis> apis);
}
