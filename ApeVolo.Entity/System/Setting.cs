using ApeVolo.Common.Model;
using ApeVolo.Entity.Base;
using SqlSugar;

namespace ApeVolo.Entity.System;

/// <summary>
/// 系统设置
/// </summary>
[SugarTable("sys_setting", "系统设置")]
public class Setting : BaseEntity, ISoftDeletedEntity
{
    [SugarColumn(ColumnDescription = "名称", IsNullable = false)]
    public string Name { get; set; }

    [SugarColumn(ColumnDescription = "值", IsNullable = false)]
    public string Value { get; set; }

    [SugarColumn(IsNullable = false)]
    public bool Enabled { get; set; }

    [SugarColumn(ColumnDescription = "描述", IsNullable = true)]
    public string Description { get; set; }

    public bool IsDeleted { get; set; }
}
