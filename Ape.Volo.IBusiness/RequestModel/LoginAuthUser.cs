using System.ComponentModel.DataAnnotations;

namespace Ape.Volo.IBusiness.RequestModel;

/// <summary>
/// 登录用户
/// </summary>
public class LoginAuthUser
{
    /// <summary>
    /// 用户名
    /// </summary>
    [Required(ErrorMessage = "{0}required")]
    public string Username { get; set; }

    /// <summary>
    /// 密码
    /// </summary>
    [Required(ErrorMessage = "{0}required")]
    public string Password { get; set; }

    /// <summary>
    /// 验证码
    /// </summary>
    [Required(ErrorMessage = "{0}required")]
    public string Captcha { get; set; }

    /// <summary>
    /// 唯一ID
    /// </summary>
    [Required(ErrorMessage = "{0}required")]
    public string CaptchaId { get; set; }
}
