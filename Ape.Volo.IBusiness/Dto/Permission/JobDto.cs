using Ape.Volo.Common.AttributeExt;
using Ape.Volo.Entity.Permission;
using Ape.Volo.IBusiness.Base;

namespace Ape.Volo.IBusiness.Dto.Permission;

[AutoMapping(typeof(Job), typeof(JobDto))]
public class JobDto : BaseEntityDto<long>
{
    public string Name { get; set; }

    public int Sort { get; set; }

    public bool Enabled { get; set; }
}
