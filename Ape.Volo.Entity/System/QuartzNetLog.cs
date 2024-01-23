using Ape.Volo.Common.Model;
using Ape.Volo.Entity.Base;
using SqlSugar;

namespace Ape.Volo.Entity.System
{
    /// <summary>
    /// 系统作业调度执行日志
    /// </summary>
    [SugarTable("sys_quartz_job_log")]
    public class QuartzNetLog : BaseEntity, ISoftDeletedEntity
    {
        /// <summary>
        /// 任务Id
        /// </summary>
        public long TaskId { get; set; }

        /// <summary>
        /// 任务名称
        /// </summary>
        public string TaskName { get; set; }

        /// <summary>
        /// 任务分组
        /// </summary>
        public string TaskGroup { get; set; }

        /// <summary>
        /// 程序集名称
        /// </summary>
        public string AssemblyName { get; set; }

        /// <summary>
        /// 任务所在类
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// cron 表达式
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Cron { get; set; }

        /// <summary>
        /// 异常详情
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string ExceptionDetail { get; set; }

        /// <summary>
        /// 执行耗时（毫秒）
        /// </summary>
        public long ExecutionDuration { get; set; }

        /// <summary>
        /// 执行传参
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string RunParams { get; set; }

        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 是否已删除
        /// </summary>
        public bool IsDeleted { get; set; }
    }
}
