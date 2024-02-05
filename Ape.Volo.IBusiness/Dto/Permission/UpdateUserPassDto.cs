using System.ComponentModel.DataAnnotations;

namespace Ape.Volo.IBusiness.Dto.Permission;

/// <summary>
/// 用户密码Dto
/// </summary>
public class UpdateUserPassDto
{
    /// <summary>
    /// 旧密码
    /// </summary>
    [Required]
    public string OldPassword { get; set; }

    /// <summary>
    /// 新密码
    /// </summary>
    [Required]
    public string NewPassword { get; set; }

    /// <summary>
    /// 确认新密码
    /// </summary>
    [Required]
    //[Compare("NewPassword", ErrorMessage = "FailedVerificationTwice")]
    public string ConfirmPassword { get; set; }
}
