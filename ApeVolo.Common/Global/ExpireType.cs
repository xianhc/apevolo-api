namespace ApeVolo.Common.Global
{
    /// <summary>
    /// 过期类型
    /// </summary>
    public enum RedisExpireType
    {
        /// <summary>
        /// 绝对过期
        /// 注：即自创建一段时间后就过期
        /// </summary>
        Absolute,

        /// <summary>
        /// 相对过期
        /// 注：即该键未被访问后一段时间后过期，若此键一直被访问则过期时间自动延长
        /// </summary>
        Relative
    }
}
