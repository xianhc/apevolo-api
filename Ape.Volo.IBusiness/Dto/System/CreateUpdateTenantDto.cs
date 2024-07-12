using System.ComponentModel.DataAnnotations;
using Ape.Volo.Common.Attributes;
using Ape.Volo.Common.Enums;
using Ape.Volo.Entity.System;
using Ape.Volo.IBusiness.Base;
using SqlSugar;

namespace Ape.Volo.IBusiness.Dto.System;

[AutoMapping(typeof(Tenant), typeof(CreateUpdateTenantDto))]
public class CreateUpdateTenantDto : BaseEntityDto<long>
{
    /// <summary>
    /// 租户Id 用户绑定用的
    /// </summary>
    public int TenantId { get; set; }

    /// <summary>
    /// 标识Id 获取租户连接用的
    /// </summary>
    public string ConfigId { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    [Required]
    public string Name { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// 租户类型
    /// </summary>
    [Required]
    public TenantType TenantType { get; set; }

    /// <summary>
    /// 数据库类型<br/>
    /// </summary>
    public DbType? DbType { get; set; }

    /// <summary>
    /// 数据库连接
    /// </summary>
    public string Connection { get; set; }
}
