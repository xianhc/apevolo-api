using System.ComponentModel.DataAnnotations;

namespace ApeVolo.IBusiness.Dto.Permission;

public class UpdateUserPassDto
{
    [Required]
    public string OldPassword { get; set; }

    [Required]
    public string NewPassword { get; set; }

    [Required]
    //[Compare("NewPassword", ErrorMessage = "FailedVerificationTwice")]
    public string ConfirmPassword { get; set; }
}
