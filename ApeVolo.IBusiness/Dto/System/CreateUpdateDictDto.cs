using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ApeVolo.Common.AttributeExt;
using ApeVolo.IBusiness.Base;

namespace ApeVolo.IBusiness.Dto.System;

[AutoMapping(typeof(Entity.System.Dict), typeof(CreateUpdateDictDto))]
public class CreateUpdateDictDto : BaseEntityDto<long>
{
    [Display(Name = "Dict.Name")]
    public string Name { get; set; }

    [Display(Name = "Dict.Description")]
    public string Description { get; set; }

    public List<CreateUpdateDictDetailDto> DictDetails { get; set; }
}
