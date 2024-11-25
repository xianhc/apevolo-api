using System.Collections.Generic;
using Ape.Volo.Common.Attributes;

namespace Ape.Volo.Common.ConfigOptions;

/// <summary>
/// 跨域配置
/// </summary>
[OptionsSettings]
public class CorsOptions
{
    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 是否全部允许
    /// </summary>
    public bool EnableAll { get; set; }

    /// <summary>
    /// 代理列表
    /// </summary>
    public List<Policy> Policy { get; set; }
}

/// <summary>
/// 代理
/// </summary>
public class Policy
{
    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 域
    /// </summary>
    public string Domain { get; set; }
}
