using System.ComponentModel.DataAnnotations;
using ApeVolo.Common.AttributeExt;
using ApeVolo.Entity.Permission;

namespace ApeVolo.IBusiness.Dto.Permission;

[AutoMapping(typeof(Department), typeof(UserDeptDto))]
public class UserDeptDto
{
    [RegularExpression(@"^\+?[1-9]\d*$")]
    public long Id { get; set; }
}
