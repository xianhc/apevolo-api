using System.ComponentModel;

namespace Ape.Volo.Common.Enums;

/// <summary>
/// 租户类型
/// </summary>
public enum TenantType
{
    /// <summary>
    /// Id隔离
    /// </summary>
    [Description("Id隔离")]
    Id = 1,

    /// <summary>
    /// 库隔离
    /// </summary>
    [Description("库隔离")]
    Db = 2
}
