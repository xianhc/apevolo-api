using System;
using SqlSugar;

namespace ApeVolo.Entity.Base;

/// <summary>
/// 泛型主键
/// </summary>
/// <typeparam name="T"></typeparam>
public class RootKey<T> where T : IEquatable<T>
{
    /// <summary>
    /// 主键
    /// </summary>
    [SugarColumn(IsPrimaryKey = true)]
    public T Id { get; set; }
}
