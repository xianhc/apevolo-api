using Ape.Volo.Common.Attributes;

namespace Ape.Volo.Common.ConfigOptions;

/// <summary>
/// Aop配置
/// </summary>
[OptionsSettings]
public class AopOptions
{
    public Tran Tran { get; set; }
    public Cache Cache { get; set; }
}

/// <summary>
/// 事务
/// </summary>
public class Tran
{
    /// <summary>
    /// 是否启用
    /// </summary>
    public bool Enabled { get; set; }
}

/// <summary>
/// 缓存
/// </summary>
public class Cache
{
    /// <summary>
    /// 是否启用
    /// </summary>
    public bool Enabled { get; set; }
}
