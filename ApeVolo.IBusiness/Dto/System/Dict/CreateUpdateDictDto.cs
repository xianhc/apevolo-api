using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ApeVolo.Common.AttributeExt;
using ApeVolo.IBusiness.Base;
using ApeVolo.IBusiness.Dto.System.Dict.Detail;

namespace ApeVolo.IBusiness.Dto.System.Dict;

[AutoMapping(typeof(Entity.System.Dictionary.Dict), typeof(CreateUpdateDictDto))]
public class CreateUpdateDictDto : EntityDtoRoot<long>
{
    [Display(Name = "Dict.Detail.Name")]
    public string Name { get; set; }

    [Display(Name = "Dict.Detail.Description")]
    public string Description { get; set; }

    public List<CreateUpdateDictDetailDto> DictDetails { get; set; }
}