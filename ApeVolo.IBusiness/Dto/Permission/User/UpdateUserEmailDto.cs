using System.ComponentModel.DataAnnotations;

namespace ApeVolo.IBusiness.Dto.Permission.User;

public class UpdateUserEmailDto
{
    [Display(Name = "User.Password")]
    [Required(ErrorMessage = "{0}required")]
    public string Password { get; set; }

    [Display(Name = "User.Email")]
    [Required(ErrorMessage = "{0}required")]
    [EmailAddress(ErrorMessage = "{0}ValueIsInvalidAccessor")]
    public string Email { get; set; }

    [Display(Name = "Captcha")]
    [Required(ErrorMessage = "{0}required")]
    public string Code { get; set; }
}