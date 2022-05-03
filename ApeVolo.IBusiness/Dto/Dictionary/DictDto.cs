using System.Collections.Generic;
using ApeVolo.Common.AttributeExt;
using ApeVolo.Entity.Do.Dictionary;

namespace ApeVolo.IBusiness.Dto.Dictionary;

[AutoMapping(typeof(Dict), typeof(DictDto))]
public class DictDto : EntityDtoRoot<long>
{
    public string Name { get; set; }

    public string Description { get; set; }

    public List<DictDetailDto> DictDetails { get; set; }
}