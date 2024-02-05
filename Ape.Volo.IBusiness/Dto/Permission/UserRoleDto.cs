using System.ComponentModel.DataAnnotations;
using Ape.Volo.Common.AttributeExt;
using Ape.Volo.Entity.Permission;

namespace Ape.Volo.IBusiness.Dto.Permission;

/// <summary>
/// 用户角色Dto
/// </summary>
[AutoMapping(typeof(Role), typeof(UserRoleDto))]
public class UserRoleDto
{
    /// <summary>
    /// ID
    /// </summary>
    [RegularExpression(@"^\+?[1-9]\d*$")]
    public long Id { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    [Required]
    public string Name { get; set; }
}
