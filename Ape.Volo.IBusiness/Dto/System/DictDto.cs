using System.Collections.Generic;
using Ape.Volo.Common.AttributeExt;
using Ape.Volo.Entity.System;
using Ape.Volo.IBusiness.Base;

namespace Ape.Volo.IBusiness.Dto.System;

[AutoMapping(typeof(Dict), typeof(DictDto))]
public class DictDto : BaseEntityDto<long>
{
    public string Name { get; set; }

    public string Description { get; set; }

    public List<DictDetailDto> DictDetails { get; set; }
}
