using System;
using SqlSugar;

namespace ApeVolo.Entity.Base;

public class RootKey<T> where T : IEquatable<T>
{
    /// <summary>
    /// 主键
    /// </summary>
    public T Id { get; set; }
}
