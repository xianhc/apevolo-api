using System.ComponentModel.DataAnnotations;
using Ape.Volo.Common.AttributeExt;
using Ape.Volo.Entity.System;
using Ape.Volo.IBusiness.Base;

namespace Ape.Volo.IBusiness.Dto.System;

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
