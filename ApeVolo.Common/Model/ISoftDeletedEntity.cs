using System;

namespace ApeVolo.Common.Model;

public interface ISoftDeletedEntity
{
    /// <summary>
    /// 是否删除
    /// </summary>
    bool IsDeleted { get; set; }
}