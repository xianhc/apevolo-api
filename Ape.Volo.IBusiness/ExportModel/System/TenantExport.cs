using System.ComponentModel.DataAnnotations;
using Ape.Volo.Common.Enums;
using Ape.Volo.Common.Model;
using SqlSugar;

namespace Ape.Volo.IBusiness.ExportModel.System;

/// <summary>
/// 租户导出模板
/// </summary>
public class TenantExport : ExportBase
{
    /// <summary>
    /// 租户Id
    /// </summary>
    [Display(Name = "租户Id")]
    public int TenantId { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    [Display(Name = "名称")]
    public string Name { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    [Display(Name = "描述")]
    public string Description { get; set; }

    /// <summary>
    /// 租户类型
    /// </summary>
    [Display(Name = "租户类型")]
    public TenantType TenantType { get; set; }

    /// <summary>
    /// 库Id
    /// </summary>
    [Display(Name = "库Id")]
    public string ConfigId { get; set; }

    /// <summary>
    /// 库类型
    /// </summary>
    [Display(Name = "库类型")]
    public DbType DbType { get; set; }

    /// <summary>
    /// 库连接
    /// </summary>
    [Display(Name = "库连接")]
    public string ConnectionString { get; set; }
}
