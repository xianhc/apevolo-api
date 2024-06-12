namespace Ape.Volo.IBusiness.QueryModel;

/// <summary>
/// 日志查询参数
/// </summary>
public class LogQueryCriteria : DateRange
{
    /// <summary>
    /// 关键字
    /// </summary>
    public string KeyWords { get; set; }
}
