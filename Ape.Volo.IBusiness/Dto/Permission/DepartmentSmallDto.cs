using Ape.Volo.Common.AttributeExt;
using Ape.Volo.Entity.Permission;

namespace Ape.Volo.IBusiness.Dto.Permission;

[AutoMapping(typeof(Department), typeof(DepartmentSmallDto))]
public class DepartmentSmallDto
{
    public string Id { get; set; }
    public string Name { get; set; }
}
