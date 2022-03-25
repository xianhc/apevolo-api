using ApeVolo.Common.AttributeExt;
using SqlSugar;

namespace ApeVolo.Entity.Do.Core;

[InitTable(typeof(Setting))]
[SugarTable("sys_setting", "系统设置表")]
public class Setting : BaseEntity
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