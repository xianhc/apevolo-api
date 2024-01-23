using System.Collections.Generic;
using System.Threading.Tasks;
using Ape.Volo.Entity.Permission;
using Ape.Volo.IBusiness.Base;
using Ape.Volo.IBusiness.Vo;

namespace Ape.Volo.IBusiness.Interface.Permission;

/// <summary>
/// 权限信息接口
/// </summary>
public interface IPermissionService : IBaseServices<Role>
{
    Task<List<PermissionVo>> QueryUserPermissionAsync(long userId);
}
