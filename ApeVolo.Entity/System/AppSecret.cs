using ApeVolo.Common.Model;
using ApeVolo.Entity.Base;
using SqlSugar;

namespace ApeVolo.Entity.System;

/// <summary>
/// 三方应用密钥
/// </summary>
[SugarTable("sys_app_secret", "三方应用密钥")]
public class AppSecret : BaseEntity, ISoftDeletedEntity
{
    /// <summary>
    /// 应用ID
    /// </summary>
    [SugarColumn(ColumnDescription = "应用ID", IsNullable = false)]
    public string AppId { get; set; }

    /// <summary>
    /// 应用秘钥
    /// </summary>
    [SugarColumn(ColumnDescription = "应用秘钥", IsNullable = false)]
    public string AppSecretKey { get; set; }

    /// <summary>
    /// 应用名称
    /// </summary>
    [SugarColumn(ColumnDescription = "应用名称", IsNullable = false)]
    public string AppName { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(ColumnDescription = "备注", IsNullable = true)]
    public string Remark { get; set; }

    public bool IsDeleted { get; set; }
}
