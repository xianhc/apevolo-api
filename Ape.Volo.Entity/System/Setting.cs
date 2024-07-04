using Ape.Volo.Common.Model;
using Ape.Volo.Entity.Base;
using SqlSugar;

namespace Ape.Volo.Entity.System;

/// <summary>
/// 系统设置
/// </summary>
[SugarTable("sys_setting")]
public class Setting : BaseEntityNoDataScope
{
    /// <summary>
    /// 名称
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public string Name { get; set; }

    /// <summary>
    /// 值
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public string Value { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public bool Enabled { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public string Description { get; set; }
}
