using System.ComponentModel.DataAnnotations;

namespace ApeVolo.IBusiness.Dto.Permission;

public class UpdateUserPassDto
{
    [Display(Name = "OldPassword")]
    [Required(ErrorMessage = "{0}required")]
    public string OldPassword { get; set; }

    [Display(Name = "NewPassword")]
    [Required(ErrorMessage = "{0}required")]
    public string NewPassword { get; set; }

    [Display(Name = "ConfirmPassword")]
    [Required(ErrorMessage = "{0}required")]
    //[Compare("NewPassword", ErrorMessage = "FailedVerificationTwice")]
    public string ConfirmPassword { get; set; }
}
