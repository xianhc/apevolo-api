using ApeVolo.Common.DI;
using ApeVolo.Common.Model;
using ApeVolo.Entity.Base;
using SqlSugar;

namespace ApeVolo.Entity.Test;

/// <summary>
/// 测试用的
/// </summary>
[SugarTable("test_apevolo", "测试")]
public class TestApeVolo : BaseEntity, ISoftDeletedEntity
{
    [SugarColumn(ColumnDescription = "label")]
    public string Label { get; set; }

    [SugarColumn(ColumnDescription = "content")]
    public string Content { get; set; }

    /// <summary>
    /// 排序123
    /// </summary>
    public int? Sort { get; set; }

    public bool IsDeleted { get; set; }
}
