using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ApeVolo.Common.AttributeExt;
using ApeVolo.IBusiness.Base;

namespace ApeVolo.IBusiness.Dto.System;

[AutoMapping(typeof(Entity.System.Dict), typeof(DictDto))]
public class DictDto : BaseEntityDto<long>
{
    [Display(Name = "Dict.Name")]
    public string Name { get; set; }

    [Display(Name = "Dict.Description")]
    public string Description { get; set; }

    public List<DictDetailDto> DictDetails { get; set; }
}
