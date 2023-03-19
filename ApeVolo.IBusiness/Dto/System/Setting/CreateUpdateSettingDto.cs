using System.ComponentModel.DataAnnotations;
using ApeVolo.Common.AttributeExt;
using ApeVolo.IBusiness.Base;

namespace ApeVolo.IBusiness.Dto.System.Setting;

[AutoMapping(typeof(Entity.System.Setting), typeof(CreateUpdateSettingDto))]
public class CreateUpdateSettingDto : EntityDtoRoot<long>
{
    [Display(Name = "Setting.Name")]
    [Required(ErrorMessage = "{0}required")]
    public string Name { get; set; }

    [Display(Name = "Setting.Value")]
    [Required(ErrorMessage = "{0}required")]
    public string Value { get; set; }

    public bool Enabled { get; set; }

    public string Description { get; set; }
}