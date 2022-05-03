using ApeVolo.Common.DI;
using ApeVolo.Entity.Do.Base;
using SqlSugar;

namespace ApeVolo.Entity.Do.Core;

/// <summary>
/// 三方应用密钥
/// </summary>
[SugarTable("sys_app_secret", "三方应用密钥")]
public class AppSecret : EntityRoot<long>, ILocalizedTable
{
    /// <summary>
    /// 应用ID
    /// </summary>
    [SugarColumn(ColumnName = "app_id", ColumnDescription = "应用ID", ColumnDataType = "varchar", Length = 255,
        IsNullable = false)]
    public string AppId { get; set; }

    /// <summary>
    /// 应用秘钥
    /// </summary>
    [SugarColumn(ColumnName = "app_secret_key", ColumnDescription = "应用秘钥", ColumnDataType = "varchar",
        Length = 255,
        IsNullable = false)]
    public string AppSecretKey { get; set; }

    /// <summary>
    /// 应用名称
    /// </summary>
    [SugarColumn(ColumnName = "app_name", ColumnDescription = "应用名称", ColumnDataType = "varchar", Length = 255,
        IsNullable = false)]
    public string AppName { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(ColumnName = "remark", ColumnDescription = "备注", ColumnDataType = "varchar", Length = 255,
        IsNullable = true)]
    public string Remark { get; set; }
}