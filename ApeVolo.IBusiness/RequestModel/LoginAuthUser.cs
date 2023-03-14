using System.ComponentModel.DataAnnotations;

namespace ApeVolo.IBusiness.RequestModel;

public class LoginAuthUser
{
    /// <summary>
    /// 用户名
    /// </summary>
    [Display(Name = "User.Name")]
    [Required(ErrorMessage = "{0}required")]
    public string Username { get; set; }

    /// <summary>
    /// 密码
    /// </summary>
    [Display(Name = "User.Password")]
    [Required(ErrorMessage = "{0}required")]
    public string Password { get; set; }

    /// <summary>
    /// 验证码
    /// </summary>
    [Display(Name = "Captcha")]
    [Required(ErrorMessage = "{0}required")]
    public string Captcha { get; set; }

    /// <summary>
    /// 唯一ID
    /// </summary>
    [Display(Name = "CaptchaId")]
    [Required(ErrorMessage = "{0}required")]
    public string CaptchaId { get; set; }
}