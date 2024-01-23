using Ape.Volo.Common.AttributeExt;
using Ape.Volo.Entity.Permission;

namespace Ape.Volo.IBusiness.Dto.Permission;

[AutoMapping(typeof(Job), typeof(JobSmallDto))]
public class JobSmallDto
{
    public string Id { get; set; }
    public string Name { get; set; }
}
