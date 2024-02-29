using Ape.Volo.Common.Model;
using Ape.Volo.Entity.Base;
using SqlSugar;

namespace Ape.Volo.Entity.Permission;

/// <summary>
/// 
/// </summary>
[SugarTable("sys_apis")]
public class Apis : BaseEntity, ISoftDeletedEntity
{
    /// <summary>
    /// 组
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public string Group { get; set; }

    /// <summary>
    /// 路径
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public string Url { get; set; }


    /// <summary>
    /// 描述
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public string Description { get; set; }

    /// <summary>
    /// 请求方法
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public string Method { get; set; }

    /// <summary>
    /// 是否已删除
    /// </summary>
    public bool IsDeleted { get; set; }
}
