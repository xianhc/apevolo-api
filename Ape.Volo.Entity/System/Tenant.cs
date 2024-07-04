using System.Collections.Generic;
using Ape.Volo.Common.Enums;
using Ape.Volo.Common.Model;
using Ape.Volo.Entity.Base;
using Ape.Volo.Entity.Permission;
using SqlSugar;

namespace Ape.Volo.Entity.System;

/// <summary>
/// 租户
/// </summary>
[SugarTable("sys_tenant")]
public class Tenant : BaseEntity
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
    [SugarColumn(IsNullable = true)]
    public string Description { get; set; }

    /// <summary>
    /// 租户类型
    /// </summary>
    public TenantType TenantType { get; set; }

    /// <summary>
    /// 库Id
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public string ConfigId { get; set; }

    /// <summary>
    /// 数据库类型<br/>
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public DbType DbType { get; set; }

    /// <summary>
    /// 数据库连接
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public string ConnectionString { get; set; }


    #region 扩展属性

    /// <summary>
    /// 用户列表
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    [Navigate(NavigateType.OneToMany, nameof(User.TenantId), nameof(TenantId))]
    public List<User> Users { get; set; }

    #endregion
}
