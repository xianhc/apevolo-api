using System.ComponentModel.DataAnnotations;
using ApeVolo.Common.AttributeExt;
using ApeVolo.IBusiness.Base;

namespace ApeVolo.IBusiness.Dto.System.AppSecret;

[AutoMapping(typeof(Entity.Do.Core.AppSecret), typeof(CreateUpdateAppSecretDto))]
public class CreateUpdateAppSecretDto : EntityDtoRoot<long>
{
    public string AppId { get; set; }

    public string AppSecretKey { get; set; }

    [Display(Name = "AppSecret.Name")]
    [Required(ErrorMessage = "{0}required")]
    public string AppName { get; set; }

    public string Remark { get; set; }
}