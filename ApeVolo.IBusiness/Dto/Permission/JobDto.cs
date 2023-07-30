using ApeVolo.Common.AttributeExt;
using ApeVolo.IBusiness.Base;

namespace ApeVolo.IBusiness.Dto.Permission;

[AutoMapping(typeof(Entity.Permission.Job), typeof(JobDto))]
public class JobDto : BaseEntityDto<long>
{
    public string Name { get; set; }

    public int Sort { get; set; }

    public bool Enabled { get; set; }
}
