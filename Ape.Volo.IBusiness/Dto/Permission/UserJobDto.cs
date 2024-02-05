using System.ComponentModel.DataAnnotations;
using Ape.Volo.Common.AttributeExt;
using Ape.Volo.Entity.Permission;

namespace Ape.Volo.IBusiness.Dto.Permission;

/// <summary>
/// 用户岗位Dto
/// </summary>
[AutoMapping(typeof(Job), typeof(UserJobDto))]
public class UserJobDto
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
