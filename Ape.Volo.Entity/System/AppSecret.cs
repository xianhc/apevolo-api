using Ape.Volo.Common.Model;
using Ape.Volo.Entity.Base;
using SqlSugar;

namespace Ape.Volo.Entity.System;

/// <summary>
/// 三方应用密钥
/// </summary>
[SugarTable("sys_app_secret")]
public class AppSecret : BaseEntity
{
    /// <summary>
    /// 应用ID
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public string AppId { get; set; }

    /// <summary>
    /// 应用秘钥
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public string AppSecretKey { get; set; }

    /// <summary>
    /// 应用名称
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public string AppName { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public string Remark { get; set; }
}
