using System.Collections.Generic;

namespace Ape.Volo.IBusiness.Vo;

/// <summary>
/// 菜单视图  构建菜单树
/// </summary>
public class MenuTreeVo
{
    public string Name { get; set; }
    public string Path { get; set; }
    public bool Hidden { get; set; }
    public string Redirect { get; set; }
    public string Component { get; set; }
    public bool AlwaysShow { get; set; }
    public List<MenuTreeVo> Children { get; set; }

    public MenuMetaVO Meta;
}
