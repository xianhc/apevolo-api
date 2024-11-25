using Ape.Volo.Common.Attributes;

namespace Ape.Volo.Common.ConfigOptions;

/// <summary>
/// RSA密钥配置
/// </summary>
[OptionsSettings]
public class RsaOptions
{
    /// <summary>
    /// 私钥
    /// </summary>
    public string PrivateKey { get; set; }

    /// <summary>
    /// 公钥
    /// </summary>
    public string PublicKey { get; set; }
}
