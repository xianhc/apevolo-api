using ApeVolo.Common.DI;
using ApeVolo.Entity.Do.Base;
using SqlSugar;

namespace ApeVolo.Entity.Do.Core;

/// <summary>
/// 测试用的
/// </summary>
[SugarTable("test_apevolo", "测试")]
public class TestApeVolo : EntityRoot<long>, ILocalizedTable
{
    [SugarColumn(ColumnName = "label", IsNullable = false, ColumnDescription = "label")]
    public string Label { get; set; }

    [SugarColumn(ColumnName = "content", IsNullable = true, ColumnDescription = "content")]
    public string Content { get; set; }

    [SugarColumn(ColumnName = "sort", IsNullable = true, ColumnDescription = "排序")]
    public int Sort { get; set; }
}