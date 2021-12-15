using System.Collections.Generic;
using System.Threading.Tasks;
using ApeVolo.Entity.Do.Core;
using ApeVolo.IBusiness.Base;
using ApeVolo.IBusiness.Vo;

namespace ApeVolo.IBusiness.Interface.Core
{
    /// <summary>
    /// 权限信息接口
    /// </summary>
    public interface IPermissionService : IBaseServices<Role>
    {
        Task<List<PermissionVO>> QueryUserPermissionAsync(long userId);
    }
}
