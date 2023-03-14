using ApeVolo.Common.AttributeExt;

namespace ApeVolo.IBusiness.Dto.Permission.Job;

[AutoMapping(typeof(Entity.Do.Core.Job), typeof(JobSmallDto))]
public class JobSmallDto
{
    public string Id { get; set; }
    public string Name { get; set; }
}