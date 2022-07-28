using System.Collections.Generic;
using ApeVolo.Common.DI;
using ApeVolo.Entity.Do.Base;
using SqlSugar;

namespace ApeVolo.Entity.Do.Dictionary;

/// <summary>
/// 字典
/// </summary>
[SugarTable("sys_dict", "字典")]
public class Dict : EntityRoot<long>, ILocalizedTable
{
    /// <summary>
    /// 字典名称
    /// </summary>
    /// <returns></returns>
    [SugarColumn(ColumnName = "name", IsNullable = false, ColumnDescription = "字典名称")]
    public string Name { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    [SugarColumn(ColumnName = "description", IsNullable = false, ColumnDescription = "描述")]
    public string Description { get; set; }

    [SugarColumn(IsIgnore = true)] public List<DictDetail> DictDetails { get; set; }
}