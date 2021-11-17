using System;
using System.Collections.Generic;

namespace ApeVolo.Common.Caches.Redis.Models
{
    /// <summary>
    /// Redis配置项
    /// </summary>
    public class RedisOptions
    {
        /// <summary>
        /// 数据库地址
        /// </summary>
        public string RedisHost { get; set; }

        /// <summary>
        /// 端口
        /// </summary>
        public string Port { get; set; }
        /// <summary>
        /// 数据库用户名
        /// </summary>
        public string RedisName { get; set; }
        /// <summary>
        /// 数据库密码
        /// </summary>
        public string RedisPass { get; set; }

        /// <summary>
        /// 库
        /// </summary>
        public int RedisIndex { get; set; }

        /// <summary>
        /// 异步连接等待时间
        /// </summary>
        public int ConnectTimeout { get; set; }

        /// <summary>
        /// 同步连接等待时间
        /// </summary>
        public int SyncTimeout { get; set; }

        /// <summary>
        /// 最大连接数
        /// </summary>
        public int KeepAlive { get; set; }

        /// <summary>
        /// 连接重试次数
        /// </summary>
        public int ConnectRetry { get; set; }

        /// <summary>
        /// 获取或设置是否应显式通知连接/配置超时通过TimeoutException
        /// </summary>
        public bool AbortOnConnectFail { get; set; }

        /// <summary>
        /// 是否允许管理员操作
        /// </summary>
        public bool AllowAdmin { get; set; }

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
}
