using Ape.Volo.Common.AttributeExt;

namespace Ape.Volo.IBusiness.Dto.Permission;

[AutoMapping(typeof(ApeVolo.Entity.Permission.Department), typeof(DepartmentSmallDto))]
public class DepartmentSmallDto
{
    public string Id { get; set; }
    public string Name { get; set; }
}
