using System.ComponentModel.DataAnnotations;
using ApeVolo.Common.AttributeExt;
using ApeVolo.Entity.Permission;

namespace ApeVolo.IBusiness.Dto.Permission;

[AutoMapping(typeof(Job), typeof(UserJobDto))]
public class UserJobDto
{
    [RegularExpression(@"^\+?[1-9]\d*$")]
    public long Id { get; set; }

    [Required]
    public string Name { get; set; }
}
