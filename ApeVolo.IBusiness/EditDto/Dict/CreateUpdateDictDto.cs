using System.Collections.Generic;
using ApeVolo.Common.AttributeExt;

namespace ApeVolo.IBusiness.EditDto.Dict;

[AutoMapping(typeof(Entity.Do.Dictionary.Dict), typeof(CreateUpdateDictDto))]
public class CreateUpdateDictDto : BaseCreateUpdateEntityDto
{
    public string Name { get; set; }

    public string Description { get; set; }

    public List<CreateUpdateDictDetailDto> DictDetails { get; set; }
}