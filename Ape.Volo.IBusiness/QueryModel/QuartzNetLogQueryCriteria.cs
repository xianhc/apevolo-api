namespace Ape.Volo.IBusiness.QueryModel;

/// <summary>
/// 任务调度日志查询参数
/// </summary>
public class QuartzNetLogQueryCriteria : DateRange
{
    /// <summary>
    /// 任务ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 是否执行成功
    /// </summary>
    public bool? IsSuccess { get; set; }
}
