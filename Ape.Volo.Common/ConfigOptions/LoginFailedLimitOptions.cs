using Ape.Volo.Common.Attributes;

namespace Ape.Volo.Common.ConfigOptions;

/// <summary>
/// 登录失败限制配置
/// </summary>
[OptionsSettings]
public class LoginFailedLimitOptions
{
    public bool Enabled { get; set; }

    /// <summary>
    /// 最大尝试次数
    /// </summary>
    public int MaxAttempts { get; set; }

    /// <summary>
    /// 锁定时间
    /// </summary>
    public int Lockout { get; set; }
}
