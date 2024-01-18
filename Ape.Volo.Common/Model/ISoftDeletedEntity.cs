namespace Ape.Volo.Common.Model;

public interface ISoftDeletedEntity
{
    /// <summary>
    /// 是否删除
    /// </summary>
    bool IsDeleted { get; set; }
}
