using ApeVolo.Common.AttributeExt;

namespace ApeVolo.IBusiness.Dto.Permission.Department;

[AutoMapping(typeof(Entity.Do.Core.Department), typeof(DepartmentSmallDto))]
public class DepartmentSmallDto
{
    public string Id { get; set; }
    public string Name { get; set; }
}