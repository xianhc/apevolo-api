using Ape.Volo.Common.Model;
using Ape.Volo.Entity.Base;
using SqlSugar;

namespace Ape.Volo.Entity.Permission;

/// <summary>
/// 岗位
/// </summary>
[SugarTable("sys_job")]
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
    [SugarColumn(IsNullable = true)]
    public int Sort { get; set; }

    /// <summary>
    /// 是否激活
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public bool Enabled { get; set; }

    /// <summary>
    /// 是否已删除
    /// </summary>
    public bool IsDeleted { get; set; }
}
