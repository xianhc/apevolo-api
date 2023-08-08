using System.Collections.Generic;
using ApeVolo.Common.Model;
using ApeVolo.Entity.Base;
using SqlSugar;

namespace ApeVolo.Entity.System;

/// <summary>
/// 字典
/// </summary>
[SugarTable("sys_dict")]
public class Dict : BaseEntity, ISoftDeletedEntity
{
    /// <summary>
    /// 字典名称
    /// </summary>
    /// <returns></returns>
    [SugarColumn(IsNullable = false)]
    public string Name { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public string Description { get; set; }

    /// <summary>
    /// 字典详情
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public List<DictDetail> DictDetails { get; set; }

    /// <summary>
    /// 是否已删除
    /// </summary>
    public bool IsDeleted { get; set; }
}
