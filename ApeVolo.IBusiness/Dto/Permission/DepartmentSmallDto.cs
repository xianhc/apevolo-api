using ApeVolo.Common.AttributeExt;

namespace ApeVolo.IBusiness.Dto.Permission;

[AutoMapping(typeof(Entity.Permission.Department), typeof(DepartmentSmallDto))]
public class DepartmentSmallDto
{
    public string Id { get; set; }
    public string Name { get; set; }
}
