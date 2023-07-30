using System.ComponentModel.DataAnnotations;
using ApeVolo.Common.AttributeExt;
using ApeVolo.Entity.System;
using ApeVolo.IBusiness.Base;

namespace ApeVolo.IBusiness.Dto.System;

[AutoMapping(typeof(DictDetail), typeof(CreateUpdateDictDetailDto))]
public class CreateUpdateDictDetailDto : BaseEntityDto<long>
{
    public string DictId { get; set; }

    [Required]
    public string Label { get; set; }

    [Required]
    public string Value { get; set; }

    public string DictSort { get; set; }

    public DictDto2 dict { get; set; }
}
