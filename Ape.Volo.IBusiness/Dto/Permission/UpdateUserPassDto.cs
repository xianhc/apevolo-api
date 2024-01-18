using System.ComponentModel.DataAnnotations;

namespace Ape.Volo.IBusiness.Dto.Permission;

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
