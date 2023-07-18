using System.Collections.Generic;
using ApeVolo.Common.DI;
using ApeVolo.Common.Model;
using ApeVolo.Entity.Base;
using SqlSugar;

namespace ApeVolo.Entity.System;

/// <summary>
/// 字典
/// </summary>
[SugarTable("sys_dict", "字典")]
public class Dict : BaseEntity, ISoftDeletedEntity
{
    /// <summary>
    /// 字典名称
    /// </summary>
    /// <returns></returns>
    [SugarColumn(IsNullable = false, ColumnDescription = "字典名称")]
    public string Name { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "描述")]
    public string Description { get; set; }

    [SugarColumn(IsIgnore = true)]
    public List<DictDetail> DictDetails { get; set; }

    public bool IsDeleted { get; set; }
}
