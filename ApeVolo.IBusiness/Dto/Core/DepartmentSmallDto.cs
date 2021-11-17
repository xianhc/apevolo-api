using ApeVolo.Common.AttributeExt;
using ApeVolo.Entity.Do.Core;

namespace ApeVolo.IBusiness.Dto.Core
{
    [AutoMapping(typeof(Department), typeof(DepartmentSmallDto))]
    public class DepartmentSmallDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
