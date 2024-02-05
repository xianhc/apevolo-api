using System.Collections.Generic;

namespace Ape.Volo.IBusiness.Vo;

/// <summary>
/// 菜单视图  构建菜单树
/// </summary>
public class MenuTreeVo
{
    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 路径
    /// </summary>
    public string Path { get; set; }

    /// <summary>
    /// 印象
    /// </summary>
    public bool Hidden { get; set; }

    /// <summary>
    /// 重定向
    /// </summary>
    public string Redirect { get; set; }

    /// <summary>
    /// 组件
    /// </summary>
    public string Component { get; set; }

    /// <summary>
    /// 总是显示
    /// </summary>
    public bool AlwaysShow { get; set; }

    /// <summary>
    /// 子节点
    /// </summary>
    public List<MenuTreeVo> Children { get; set; }

    /// <summary>
    /// Meta
    /// </summary>
    public MenuMetaVO Meta;
}
