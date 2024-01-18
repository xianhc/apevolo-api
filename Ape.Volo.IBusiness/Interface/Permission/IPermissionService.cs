using System.Collections.Generic;
using System.Threading.Tasks;
using Ape.Volo.IBusiness.Base;
using Ape.Volo.IBusiness.Vo;
using ApeVolo.Entity.Permission;

namespace Ape.Volo.IBusiness.Interface.Permission;

/// <summary>
/// 权限信息接口
/// </summary>
public interface IPermissionService : IBaseServices<Role>
{
    Task<List<PermissionVo>> QueryUserPermissionAsync(long userId);
}
