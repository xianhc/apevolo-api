using System.ComponentModel.DataAnnotations;
using ApeVolo.IBusiness.Base;

namespace ApeVolo.IBusiness.Dto.Permission;

public class UpdateUserCenterDto
{
    [RegularExpression(@"^\+?[1-9]\d*$", ErrorMessage = "{0}required")]
    public long Id { get; set; }

    [Display(Name = "User.NickName")]
    [Required(ErrorMessage = "{0}required")]
    public string NickName { get; set; }

    [Display(Name = "User.Gender")]
    [Required(ErrorMessage = "{0}required")]
    public string Gender { get; set; }

    [Display(Name = "User.Phone")]
    [Required(ErrorMessage = "{0}required")]
    [RegularExpression(@"^(13[0-9]|14[01456879]|15[0-35-9]|16[2567]|17[0-8]|18[0-9]|19[0-35-9])\d{8}$",
        ErrorMessage = "{0}ValueIsInvalidAccessor")]
    public string Phone { get; set; }
}
