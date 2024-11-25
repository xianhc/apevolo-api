using Ape.Volo.Common.Attributes;

namespace Ape.Volo.Common.ConfigOptions;

/// <summary>
/// JWT配置
/// </summary>
[OptionsSettings]
public class JwtAuthOptions
{
    /// <summary>
    /// 听众
    /// </summary>
    public string Audience { get; set; }

    /// <summary>
    /// 发行者
    /// </summary>
    public string Issuer { get; set; }

    /// <summary>
    /// 签名密钥
    /// </summary>
    public string SecurityKey { get; set; }

    /// <summary>
    /// 过期时间 h(小时)
    /// </summary>
    public int Expires { get; set; }

    /// <summary>
    /// 允许Token刷新时间 h(小时)
    /// </summary>
    public int RefreshTokenExpires { get; set; }

    /// <summary>
    /// 登录路径
    /// </summary>
    public string LoginPath { get; set; }
}
