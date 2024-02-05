using System.Collections.Generic;
using Ape.Volo.IBusiness.Dto.Permission;

namespace Ape.Volo.IBusiness.Vo;

/// <summary>
/// JWT令牌用户
/// </summary>
public class JwtUserVo
{
    /// <summary>
    /// 用户
    /// </summary>
    public UserDto User { get; set; }

    /// <summary>
    /// 角色权限
    /// </summary>
    public List<string> Roles { get; set; }

    /// <summary>
    /// 部门权限
    /// </summary>
    public List<string> DataScopes { get; set; }
}
