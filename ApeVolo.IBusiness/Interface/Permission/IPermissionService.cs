using System.Collections.Generic;
using System.Threading.Tasks;
using ApeVolo.IBusiness.Base;
using ApeVolo.IBusiness.Vo;

namespace ApeVolo.IBusiness.Interface.Permission;

/// <summary>
/// 权限信息接口
/// </summary>
public interface IPermissionService : IBaseServices<Entity.Permission.Role>
{
    Task<List<PermissionVo>> QueryUserPermissionAsync(long userId);
}
