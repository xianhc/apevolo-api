using System.ComponentModel.DataAnnotations;
using Ape.Volo.Common.AttributeExt;
using ApeVolo.Entity.Permission;

namespace Ape.Volo.IBusiness.Dto.Permission;

[AutoMapping(typeof(Menu), typeof(RoleMenuDto))]
public class RoleMenuDto
{
    [RegularExpression(@"^\+?[1-9]\d*$")]
    public long Id { get; set; }
}
