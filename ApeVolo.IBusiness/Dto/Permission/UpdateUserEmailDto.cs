using System.ComponentModel.DataAnnotations;

namespace ApeVolo.IBusiness.Dto.Permission;

public class UpdateUserEmailDto
{
    [Required]
    public string Password { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Code { get; set; }
}
