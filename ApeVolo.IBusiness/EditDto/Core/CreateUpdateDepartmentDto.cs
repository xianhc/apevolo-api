using ApeVolo.Common.AttributeExt;
using ApeVolo.Entity.Do.Core;

namespace ApeVolo.IBusiness.EditDto.Core
{
    [AutoMapping(typeof(Department), typeof(CreateUpdateDepartmentDto))]
    public class CreateUpdateDepartmentDto : BaseCreateUpdateEntityDto
    {
        [ApeVoloRequired(Message = "部门名称不能为空！")]
        public string Name { get; set; }
        public long? PId { get; set; }
        public int Sort { get; set; }
        public bool Enabled { get; set; }
        //public int SubCount { get; set; }
    }
}
