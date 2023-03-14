using System;
using System.ComponentModel.DataAnnotations;

namespace ApeVolo.IBusiness.Base;

public abstract class RootId<T> where T : IEquatable<T>
{
    [RegularExpression(@"^\+?[1-9]\d*$", ErrorMessage = "{0}required")]
    public T Id { get; set; }
}