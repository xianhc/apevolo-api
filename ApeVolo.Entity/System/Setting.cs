using ApeVolo.Common.Model;
using ApeVolo.Entity.Base;
using SqlSugar;

namespace ApeVolo.Entity.System;

/// <summary>
/// 系统设置
/// </summary>
[SugarTable("sys_setting")]
public class Setting : BaseEntity, ISoftDeletedEntity
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

    /// <summary>
    /// 是否已删除
    /// </summary>
    public bool IsDeleted { get; set; }
}
