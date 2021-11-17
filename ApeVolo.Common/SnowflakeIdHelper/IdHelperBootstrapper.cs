namespace ApeVolo.Common.SnowflakeIdHelper
{
    /// <summary>
    /// 配置引导
    /// </summary>
    public sealed class IdHelperBootstrapper
    {
        /// <summary>
        /// 机器Id
        /// </summary>
        private long _worderId { get; set; }

        /// <summary>
        /// 获取机器Id
        /// </summary>
        /// <returns></returns>
        private long GetWorkerId()
        {
            return _worderId;
        }

        /// <summary>
        /// 设置机器Id
        /// </summary>
        /// <param name="workderId">机器Id</param>
        /// <returns></returns>
        public IdHelperBootstrapper SetWorkderId(long workderId)
        {
            _worderId = workderId;

            return this;
        }

        /// <summary>
        /// 完成配置
        /// </summary>
        public void Boot()
        {
            IdHelper.IdWorker = new SnowflakeIdWorker(GetWorkerId());
            IdHelper.IdHelperBootstrapper = this;
        }
    }
}
