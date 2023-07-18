using System;
using SqlSugar;

namespace ApeVolo.Entity.Base;

public class RootKey<T> where T : IEquatable<T>
{
    /// <summary>
    /// 主键
    /// </summary>
    [SugarColumn(ColumnDataType = "bigint", IsPrimaryKey = true, ColumnDescription = "ID主键")]
    public T Id { get; set; }
}
