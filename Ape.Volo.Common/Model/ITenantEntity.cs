namespace Ape.Volo.Common.Model;

/// <summary>
/// 租户标识(Id隔离)
/// </summary>
public interface ITenantEntity
{
    /// <summary>
    /// 租户Id
    /// </summary>
    int TenantId { get; set; }
}
