using Ape.Volo.Common.AttributeExt;

namespace Ape.Volo.IBusiness.Dto.Permission;

[AutoMapping(typeof(ApeVolo.Entity.Permission.Job), typeof(JobSmallDto))]
public class JobSmallDto
{
    public string Id { get; set; }
    public string Name { get; set; }
}
