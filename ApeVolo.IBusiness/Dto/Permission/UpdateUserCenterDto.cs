using System.ComponentModel.DataAnnotations;

namespace ApeVolo.IBusiness.Dto.Permission;

public class UpdateUserCenterDto
{
    [RegularExpression(@"^\+?[1-9]\d*$")]
    public long Id { get; set; }

    [Required]
    public string NickName { get; set; }

    [Required]
    public string Gender { get; set; }

    [Required]
    [RegularExpression(@"^(13[0-9]|14[01456879]|15[0-35-9]|16[2567]|17[0-8]|18[0-9]|19[0-35-9])\d{8}$")]
    public string Phone { get; set; }
}
