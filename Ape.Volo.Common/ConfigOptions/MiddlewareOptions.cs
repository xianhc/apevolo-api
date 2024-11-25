using Ape.Volo.Common.Attributes;

namespace Ape.Volo.Common.ConfigOptions;

/// <summary>
/// 中间件配置
/// </summary>
[OptionsSettings]
public class MiddlewareOptions
{
    /// <summary>
    /// 作业调度
    /// </summary>
    public QuartzNetJob QuartzNetJob { get; set; }

    /// <summary>
    /// Ip限流
    /// </summary>
    public IpLimit IpLimit { get; set; }

    /// <summary>
    /// 性能监控
    /// </summary>
    public MiniProfiler MiniProfiler { get; set; }

    /// <summary>
    /// Rabbit消息队列
    /// </summary>
    public RabbitMq RabbitMq { get; set; }

    /// <summary>
    /// Redis消息队列
    /// </summary>
    public RedisMq RedisMq { get; set; }

    /// <summary>
    /// Elasticsearch日志收集
    /// </summary>
    public Elasticsearch Elasticsearch { get; set; }
}

/// <summary>
/// 作业调度
/// </summary>
public class QuartzNetJob
{
    /// <summary>
    /// 是否启用
    /// </summary>
    public bool Enabled { get; set; }
}

/// <summary>
/// Ip限流
/// </summary>
public class IpLimit
{
    /// <summary>
    /// 是否启用
    /// </summary>
    public bool Enabled { get; set; }
}

/// <summary>
/// 性能监控
/// </summary>
public class MiniProfiler
{
    /// <summary>
    /// 是否启用
    /// </summary>
    public bool Enabled { get; set; }
}

/// <summary>
/// Rabbit消息队列
/// </summary>
public class RabbitMq
{
    /// <summary>
    /// 是否启用
    /// </summary>
    public bool Enabled { get; set; }
}

/// <summary>
/// Redis消息队列
/// </summary>
public class RedisMq
{
    /// <summary>
    /// 是否启用
    /// </summary>
    public bool Enabled { get; set; }
}

/// <summary>
/// Elasticsearch日志收集
/// </summary>
public class Elasticsearch
{
    /// <summary>
    /// 是否启用
    /// </summary>
    public bool Enabled { get; set; }
}
