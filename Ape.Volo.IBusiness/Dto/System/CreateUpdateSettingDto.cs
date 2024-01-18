using System.ComponentModel.DataAnnotations;
using Ape.Volo.Common.AttributeExt;
using Ape.Volo.IBusiness.Base;

namespace Ape.Volo.IBusiness.Dto.System;

[AutoMapping(typeof(ApeVolo.Entity.System.Setting), typeof(CreateUpdateSettingDto))]
public class CreateUpdateSettingDto : BaseEntityDto<long>
{
    [Required]
    public string Name { get; set; }

    [Required]
    public string Value { get; set; }

    public bool Enabled { get; set; }

    public string Description { get; set; }
}
