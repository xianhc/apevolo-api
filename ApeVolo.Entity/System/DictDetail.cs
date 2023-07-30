using ApeVolo.Common.Model;
using ApeVolo.Entity.Base;
using SqlSugar;

namespace ApeVolo.Entity.System;

/// <summary>
/// 字典详情
/// </summary>
[SugarTable("sys_dict_detail", "字典详细")]
public class DictDetail : BaseEntity, ISoftDeletedEntity
{
    /// <summary>
    /// 字典ID
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "字典ID")]
    public long DictId { get; set; }

    /// <summary>
    /// 字典标签
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "字典标签")]
    public string Label { get; set; }

    /// <summary>
    /// 字典值
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "字典值")]
    public string Value { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "排序")]
    public string DictSort { get; set; }

    public bool IsDeleted { get; set; }
}
