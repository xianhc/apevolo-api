using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Ape.Volo.Common.AttributeExt;
using Ape.Volo.IBusiness.Base;

namespace Ape.Volo.IBusiness.Dto.System;

[AutoMapping(typeof(ApeVolo.Entity.System.Dict), typeof(CreateUpdateDictDto))]
public class CreateUpdateDictDto : BaseEntityDto<long>
{
    [Required]
    public string Name { get; set; }

    [Required]
    public string Description { get; set; }

    public List<CreateUpdateDictDetailDto> DictDetails { get; set; }
}
