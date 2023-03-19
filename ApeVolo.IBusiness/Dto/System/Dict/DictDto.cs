using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ApeVolo.Common.AttributeExt;
using ApeVolo.IBusiness.Base;
using ApeVolo.IBusiness.Dto.System.Dict.Detail;

namespace ApeVolo.IBusiness.Dto.System.Dict;

[AutoMapping(typeof(Entity.System.Dictionary.Dict), typeof(DictDto))]
public class DictDto : EntityDtoRoot<long>
{
    [Display(Name = "Dict.Name")]
    public string Name { get; set; }

    [Display(Name = "Dict.Description")]
    public string Description { get; set; }

    public List<DictDetailDto> DictDetails { get; set; }
}