using ApeVolo.Common.AttributeExt;
using ApeVolo.Entity.Do.Core;

namespace ApeVolo.IBusiness.EditDto.Core;

[AutoMapping(typeof(Job), typeof(CreateUpdateJobDto))]
public class CreateUpdateJobDto : BaseCreateUpdateEntityDto
{
    public string Name { get; set; }
    public int Sort { get; set; }
    public bool Enabled { get; set; }
}