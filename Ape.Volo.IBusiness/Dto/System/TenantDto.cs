using Ape.Volo.Common.AttributeExt;
using Ape.Volo.Common.Enums;
using Ape.Volo.Entity.System;
using Ape.Volo.IBusiness.Base;
using SqlSugar;

namespace Ape.Volo.IBusiness.Dto.System;

/// <summary>
/// 租户dto
/// </summary>
[AutoMapping(typeof(Tenant), typeof(TenantDto))]
public class TenantDto : BaseEntityDto<long>
{
    /// <summary>
    /// 租户Id
    /// </summary>
    public int TenantId { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// 租户类型
    /// </summary>
    public TenantType TenantType { get; set; }

    /// <summary>
    /// 库Id
    /// </summary>
    public string ConfigId { get; set; }

    /// <summary>
    /// 库类型
    /// </summary>
    public DbType DbType { get; set; }

    /// <summary>
    /// 库连接
    /// </summary>
    public string ConnectionString { get; set; }
}
