using Ape.Volo.Common.Attributes;

namespace Ape.Volo.Common.ConfigOptions;

/// <summary>
/// Rdis配置
/// </summary>
[OptionsSettings]
public class RedisOptions
{
    /// <summary>
    /// 用于通过哨兵解析服务的服务名称。 （Sentinel模式下需要）
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 主机
    /// </summary>
    public string Host { get; set; }

    /// <summary>
    /// 端口
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    /// 密码
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// 库
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    /// 指定允许连接的时间（以毫秒为单位）（默认为 5 秒，除非 SyncTimeout 更高）。
    /// </summary>
    public int ConnectTimeout { get; set; }

    /// <summary>
    /// 指定系统应允许同步操作的时间（以毫秒为单位）（默认为 5 秒）。
    /// </summary>
    public int SyncTimeout { get; set; }

    /// <summary>
    /// 指定应对连接执行 ping 操作以确保有效性的时间（以秒为单位）（默认为 60 秒）。
    /// </summary>
    public int KeepAlive { get; set; }

    /// <summary>
    /// 如果没有服务器及时响应，则重复初始连接周期的次数。
    /// </summary>
    public int ConnectRetry { get; set; }

    /// <summary>
    /// 获取或设置是否应通过 TimeoutException 显式通知连接/配置超时。
    /// </summary>
    public bool AbortOnConnectFail { get; set; }

    /// <summary>
    /// 指示是否允许管理操作。
    /// </summary>
    public bool AllowAdmin { get; set; }

    /// <summary>
    /// 消息队列、 没消息时挂起时长(毫秒)
    /// </summary>
    public int SuspendTime { get; set; }

    /// <summary>
    /// 消息队列、 每次消费消息间隔时间(毫秒)
    /// </summary>
    public int IntervalTime { get; set; }

    /// <summary>
    /// 消息队列、 如果是批量消费 一次消费最大处理多少条
    /// </summary>
    public int MaxQueueConsumption { get; set; }

    /// <summary>
    /// 消息队列、 是否显示日志
    /// </summary>
    public bool ShowLog { get; set; }
}
