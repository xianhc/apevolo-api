using System.Collections.Generic;
using ApeVolo.IBusiness.Dto.Core;

namespace ApeVolo.IBusiness.Vo;

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