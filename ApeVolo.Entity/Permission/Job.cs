using ApeVolo.Common.Model;
using ApeVolo.Entity.Base;
using SqlSugar;

namespace ApeVolo.Entity.Permission;

/// <summary>
/// 岗位
/// </summary>
[SugarTable("sys_job", "岗位")]
public class Job : BaseEntity, ISoftDeletedEntity
{
    /// <summary>
    /// 名称
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public string Name { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "排序")]
    public int Sort { get; set; }

    /// <summary>
    /// 是否激活
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "是否激活")]
    public bool Enabled { get; set; }

    public bool IsDeleted { get; set; }
}
