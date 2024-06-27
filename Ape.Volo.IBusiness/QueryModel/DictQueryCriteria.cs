using Ape.Volo.Common.Enums;

namespace Ape.Volo.IBusiness.QueryModel;

/// <summary>
/// 字典查询参数
/// </summary>
public class DictQueryCriteria
{
    /// <summary>
    /// 关键字
    /// </summary>
    public string KeyWords { get; set; }

    /// <summary>
    /// 类型
    /// </summary>
    public DictType? DictType { get; set; }
}
