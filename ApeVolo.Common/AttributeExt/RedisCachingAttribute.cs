using ApeVolo.Common.Global;
using System;

namespace ApeVolo.Common.AttributeExt
{
    /// <summary>
    /// Redis特性  AOP拦截使用
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class RedisCachingAttribute : Attribute
    {
        /// <summary>
        /// 过期时间（分钟）
        /// </summary>
        public int Expiration { get; set; } = 20;

        /// <summary>
        /// 缓存key前缀
        /// </summary>
        public string KeyPrefix { get; set; } = "";

        /// <summary>
        /// 缓存类型（默认绝对过期）
        /// </summary>
        public RedisExpireType RedisExpireType { get; set; } = RedisExpireType.Absolute;
    }
}