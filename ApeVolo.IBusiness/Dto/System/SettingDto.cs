using System.ComponentModel.DataAnnotations;
using ApeVolo.Common.AttributeExt;
using ApeVolo.IBusiness.Base;

namespace ApeVolo.IBusiness.Dto.System;

[AutoMapping(typeof(Entity.System.Setting), typeof(SettingDto))]
public class SettingDto : BaseEntityDto<long>
{
    [Display(Name = "Setting.Name")]
    public string Name { get; set; }

    [Display(Name = "Setting.Value")]
    public string Value { get; set; }

    [Display(Name = "Setting.Enabled")]
    public bool Enabled { get; set; }

    [Display(Name = "Setting.Description")]
    public string Description { get; set; }
}
