using System;
using SqlSugar;

namespace Ape.Volo.Entity.Base;

/// <summary>
/// 泛型主键
/// </summary>
/// <typeparam name="TKey"></typeparam>
public class RootKey<TKey> where TKey : IEquatable<TKey>
{
    /// <summary>
    /// 主键
    /// </summary>
    [SugarColumn(IsPrimaryKey = true)]
    public TKey Id { get; set; }
}
