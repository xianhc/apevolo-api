using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ape.Volo.IBusiness.Interface.Permission;

/// <summary>
/// 数据权限接口
/// </summary>
public interface IDataScopeService
{
    /// <summary>
    /// 获取用户所有角色关联的部门ID
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<List<string>> GetDataScopeAccountsAsync(long userId);
}
