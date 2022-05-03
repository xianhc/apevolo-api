using ApeVolo.Common.DI;
using ApeVolo.Entity.Do.Base;
using SqlSugar;

namespace ApeVolo.Entity.Do.Core;

/// <summary>
/// 系统设置
/// </summary>
[SugarTable("sys_setting", "系统设置")]
public class Setting : EntityRoot<long>, ILocalizedTable
{
    [SugarColumn(ColumnName = "name", ColumnDescription = "名称", ColumnDataType = "varchar", Length = 255,
        IsNullable = false)]
    public string Name { get; set; }

    [SugarColumn(ColumnName = "value", ColumnDescription = "值", ColumnDataType = "varchar", Length = 500,
        IsNullable = false)]
    public string Value { get; set; }

    [SugarColumn(ColumnName = "enabled", IsNullable = false)]
    public bool Enabled { get; set; }

    [SugarColumn(ColumnName = "description", ColumnDescription = "描述", ColumnDataType = "varchar", Length = 500,
        IsNullable = true)]
    public string Description { get; set; }
}