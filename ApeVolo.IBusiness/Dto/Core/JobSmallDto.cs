using ApeVolo.Common.AttributeExt;
using ApeVolo.Entity.Do.Core;

namespace ApeVolo.IBusiness.Dto.Core
{
    [AutoMapping(typeof(Job), typeof(JobSmallDto))]
    public class JobSmallDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
