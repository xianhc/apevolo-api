using Ape.Volo.Common.Model;
using ApeVolo.Entity.Base;
using SqlSugar;

namespace ApeVolo.Entity.Test;

/// <summary>
/// 测试
/// </summary>
[SugarTable("test_apevolo")]
public class TestApeVolo : BaseEntity, ISoftDeletedEntity
{
    public string Label { get; set; }

    public string Content { get; set; }

    /// <summary>
    /// 排序123
    /// </summary>
    public int? Sort { get; set; }

    public bool IsDeleted { get; set; }
}
