using Ape.Volo.Common.Enums;
using SqlSugar;

namespace Ape.Volo.IBusiness.QueryModel;

/// <summary>
/// 租户查询参数
/// </summary>
public class TenantQueryCriteria : DateRange
{
    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 租户类型
    /// </summary>
    public TenantType? TenantType { get; set; }

    /// <summary>
    /// 数据库类型<br/>
    /// </summary>
    public DbType? DbType { get; set; }
}
