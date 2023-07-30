using System.ComponentModel.DataAnnotations;
using ApeVolo.Common.AttributeExt;
using ApeVolo.IBusiness.Base;

namespace ApeVolo.IBusiness.Dto.System;

[AutoMapping(typeof(Entity.System.Setting), typeof(CreateUpdateSettingDto))]
public class CreateUpdateSettingDto : BaseEntityDto<long>
{
    [Required]
    public string Name { get; set; }
    
    [Required]
    public string Value { get; set; }

    public bool Enabled { get; set; }

    public string Description { get; set; }
}
