using System;
using System.Collections.Generic;

namespace Ape.Volo.Common.Caches.Redis.Models;

/// <summary>
/// Redis配置项
/// </summary>
public class RedisQueueOptions
{
    #region 消息队列配置

    /// <summary>
    /// 没消息时挂起时长(毫秒)
    /// </summary>
    public int SuspendTime { get; set; }

    /// <summary>
    /// 每次消费消息间隔时间(毫秒)
    /// </summary>
    public int IntervalTime { get; set; }

    /// <summary>
    /// 是否显示日志
    /// </summary>
    public bool ShowLog { get; set; }

    /// <summary>
    /// 需要注入的类型
    /// </summary>
    public IList<Type> ListSubscribe { get; set; }

    #endregion
}
