using ApeVolo.Common.AttributeExt;
using ApeVolo.Entity.Do.Core;

namespace ApeVolo.IBusiness.Dto.Core
{
    [AutoMapping(typeof(Job), typeof(JobDto))]
    public class JobDto : BaseEntityDto
    {
        public string Name { get; set; }
        public int Sort { get; set; }
        public bool Enabled { get; set; }
    }
}
