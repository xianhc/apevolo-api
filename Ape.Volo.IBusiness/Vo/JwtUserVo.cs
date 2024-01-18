using System.Collections.Generic;
using Ape.Volo.IBusiness.Dto.Permission;

namespace Ape.Volo.IBusiness.Vo;

/// <summary>
/// JWT令牌用户
/// </summary>
public class JwtUserVo
{
    //public List<RoleSmallDto> Roles { get; set; }
    public UserDto User { get; set; }

    public List<string> Roles { get; set; }

    //public DepartmentSmallDto Dept { get; set; }

    // public List<JobSmallDto> Jobs { get; set; }

    public List<string> DataScopes { get; set; }
}
