using ApeVolo.IBusiness.Base;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApeVolo.Entity.Do.Core;

namespace ApeVolo.IBusiness.Interface.Core
{
    /// <summary>
    /// 角色菜单
    /// </summary>
    public interface IRolesMenusService : IBaseServices<RoleMenu>
    {
        #region 接触接口
        
        Task<bool> CreateAsync(List<RoleMenu> roleMenu);
        
        Task<bool> DeleteAsync(List<string> roleIds);

        #endregion
    }
}