using System.ComponentModel.DataAnnotations;
using ApeVolo.Common.AttributeExt;
using ApeVolo.IBusiness.Base;

namespace ApeVolo.IBusiness.Dto.System.AppSecret;

[AutoMapping(typeof(Entity.System.AppSecret), typeof(AppSecretDto))]
public class AppSecretDto : EntityDtoRoot<long>
{
    [Display(Name = "App.AppId")]
    public string AppId { get; set; }

    [Display(Name = "App.AppSecretKey")]
    public string AppSecretKey { get; set; }

    [Display(Name = "App.AppName")]
    public string AppName { get; set; }

    [Display(Name = "App.Remark")]
    public string Remark { get; set; }
}