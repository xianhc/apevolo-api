using System.Collections.Generic;

namespace Ape.Volo.Common.Model;

public class ApisTree
{
    public long Id { get; set; }

    /// <summary>
    /// 标签
    /// </summary>
    public string Label { get; set; }

    /// <summary>
    /// 页
    /// </summary>
    public bool Leaf { get; set; }

    /// <summary>
    /// 是否有子节点
    /// </summary>
    public bool HasChildren { get; set; }

    /// <summary>
    /// 子节点
    /// </summary>
    public List<ApisTree> Children { get; set; }
}
