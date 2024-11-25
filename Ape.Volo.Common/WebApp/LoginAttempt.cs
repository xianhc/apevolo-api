using System;

namespace Ape.Volo.Common.WebApp;

/// <summary>
/// 登录尝试
/// </summary>
public class LoginAttempt
{
    /// <summary>
    /// 次数
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    /// 是否锁定
    /// </summary>
    public bool IsLocked { get; set; }

    /// <summary>
    /// 解锁时间
    /// </summary>
    public DateTime LockUntil { get; set; }
}
