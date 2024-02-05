using System.ComponentModel.DataAnnotations;

namespace Ape.Volo.IBusiness.Dto.Permission;

/// <summary>
/// 用户个人中心Dto
/// </summary>
public class UpdateUserCenterDto
{
    /// <summary>
    /// ID
    /// </summary>
    [RegularExpression(@"^\+?[1-9]\d*$")]
    public long Id { get; set; }

    /// <summary>
    /// 昵称
    /// </summary>
    [Required]
    public string NickName { get; set; }

    /// <summary>
    /// 性别
    /// </summary>
    [Required]
    public string Gender { get; set; }

    /// <summary>
    /// 电话
    /// </summary>
    [Required]
    [RegularExpression(@"^(13[0-9]|14[01456879]|15[0-35-9]|16[2567]|17[0-8]|18[0-9]|19[0-35-9])\d{8}$")]
    public string Phone { get; set; }
}
