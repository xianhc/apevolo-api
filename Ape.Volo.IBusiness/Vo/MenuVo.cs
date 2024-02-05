using System.Collections.Generic;

namespace Ape.Volo.IBusiness.Vo;

/// <summary>
/// 菜单视图 下拉框使用
/// </summary>
public class MenuVo
{
    /// <summary>
    /// ID
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 表i
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// 路径
    /// </summary>
    public string Path { get; set; }

    /// <summary>
    /// IFrame
    /// </summary>
    public bool Iframe { get; set; }

    /// <summary>
    /// 组件
    /// </summary>
    public string Component { get; set; }

    /// <summary>
    /// 组件名称
    /// </summary>
    public string ComponentName { get; set; }

    /// <summary>
    /// 父级ID
    /// </summary>
    public string PId { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    public int Sort { get; set; }

    /// <summary>
    /// Icon
    /// </summary>
    public string Icon { get; set; }

    /// <summary>
    /// 类型
    /// </summary>
    public int Type { get; set; }

    /// <summary>
    /// 缓存
    /// </summary>
    public bool Cache { get; set; }

    /// <summary>
    /// 隐藏
    /// </summary>
    public bool Hidden { get; set; }

    /// <summary>
    /// 子菜单个数
    /// </summary>
    public int SubCount { get; set; }

    /// <summary>
    /// 子节点
    /// </summary>
    public List<MenuVo> Children { get; set; }

    /// <summary>
    /// 叶
    /// </summary>
    public bool Leaf { get; set; }

    /// <summary>
    /// 是否有子字节
    /// </summary>
    public bool HasChildren { get; set; }

    /// <summary>
    /// 标题
    /// </summary>
    public string Label { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public string CreateTime { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public string UpdateTime { get; set; }
}
