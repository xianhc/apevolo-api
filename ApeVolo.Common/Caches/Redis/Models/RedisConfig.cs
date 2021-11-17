namespace ApeVolo.Common.Caches.Redis.Models
{
    /// <summary>
    /// 缓存数据库模型
    /// </summary>
    public class RedisConfig
    {
        /// <summary>
        /// 数据库地址
        /// </summary>
        public string RedisHost { get; set; }
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
    }
}
