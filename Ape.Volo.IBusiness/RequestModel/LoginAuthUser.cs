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
    [Required]
    public string Username { get; set; }

    /// <summary>
    /// 密码
    /// </summary>
    [Required]
    public string Password { get; set; }

    /// <summary>
    /// 验证码
    /// </summary>
    //[Required]
    public string Captcha { get; set; }

    /// <summary>
    /// 唯一ID
    /// </summary>
    [Required]
    public string CaptchaId { get; set; }
}

/// <summary>
/// Swagger登录用户
/// </summary>
public class SwaggerLoginAuthUser
{
    /// <summary>
    /// 用户名
    /// </summary>
    [Required]
    public string Username { get; set; }

    /// <summary>
    /// 密码
    /// </summary>
    [Required]
    public string Password { get; set; }
}
