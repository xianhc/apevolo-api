using ApeVolo.Common.DI;
using ApeVolo.Entity.Base;
using SqlSugar;

namespace ApeVolo.Entity.Permission;

/// <summary>
/// 岗位
/// </summary>
[SugarTable("sys_job", "岗位")]
public class Job : EntityRoot<long>, ILocalizedTable
{
    /// <summary>
    /// 名称
    /// </summary>
    [SugarColumn(ColumnName = "name", IsNullable = false)]
    public string Name { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    [SugarColumn(ColumnName = "sort", IsNullable = true, ColumnDescription = "排序")]
    public int Sort { get; set; }

    /// <summary>
    /// 是否激活
    /// </summary>
    [SugarColumn(ColumnName = "enabled", IsNullable = false, ColumnDescription = "是否激活")]
    public bool Enabled { get; set; }
}