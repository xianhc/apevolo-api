namespace ApeVolo.Common.Helper
{
    /// <summary>
    /// 时间帮助类
    /// </summary>
    public static class JobHelper
    {
        #region 私有成员

        //static JobHelper()
        //{
        //    _scheduler = TaskHelper.RunSync(() => StdSchedulerFactory.GetDefaultScheduler());
        //    TaskHelper.RunSync(() => _scheduler.Start());
        //}

        //private static ConcurrentDictionary<string, object> _store { get; }
        //    = new ConcurrentDictionary<string, object>();

        //private static readonly IScheduler _scheduler;

        //static ConcurrentDictionary<string, Action> _jobs { get; }
        //    = new ConcurrentDictionary<string, Action>();

        #endregion

        #region 外部接口

        
        //public static string SetIntervalJob(Action action, TimeSpan timeSpan)
        //{
        //    string key = Guid.NewGuid().ToString();
        //    Timer threadTimer = new Timer((state =>
        //    {
        //        action.Invoke();
        //    }), null, 0, (long)timeSpan.TotalMilliseconds);

        //    _store[key] = threadTimer;

        //    return key;
        //}

        //public static void SetDailyJob(Action action, int h, int m, int s)
        //{
        //    string key = Guid.NewGuid().ToString();
        //    _jobs[key] = action;
        //    IJobDetail job = JobBuilder.Create<Job>()
        //       .WithIdentity(key)
        //       .Build();
        //    ITrigger trigger = TriggerBuilder.Create()
        //        .WithIdentity(key)
        //        .StartNow()
        //        .WithCronSchedule($"{s} {m} {h} * * ?")//每天定时
        //        .Build();
        //    TaskHelper.RunSync(() => _scheduler.ScheduleJob(job, trigger));
        //}

        #endregion

        #region 内部类

        //class Job : IJob
        //{
        //    public Task Execute(IJobExecutionContext context)
        //    {
        //        return Task.Run(() =>
        //        {
        //            string jobName = context.JobDetail.Key.Name;
        //            if (_jobs.ContainsKey(jobName))
        //            {
        //                _jobs[jobName]?.Invoke();
        //            }
        //        });
        //    }
        //}

        #endregion
    }
}
