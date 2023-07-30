using System.Collections.Generic;
using ApeVolo.Common.AttributeExt;
using ApeVolo.IBusiness.Base;

namespace ApeVolo.IBusiness.Dto.System;

[AutoMapping(typeof(Entity.System.Dict), typeof(DictDto))]
public class DictDto : BaseEntityDto<long>
{
    public string Name { get; set; }

    public string Description { get; set; }

    public List<DictDetailDto> DictDetails { get; set; }
}
