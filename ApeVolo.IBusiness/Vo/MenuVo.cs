using System.Collections.Generic;

namespace ApeVolo.IBusiness.Vo;

/// <summary>
/// 菜单视图 下拉框使用
/// </summary>
public class MenuVo
{
    public string Id { get; set; }
    public string Title { get; set; }

    public string Path { get; set; }

    public bool Iframe { get; set; }

    public string Component { get; set; }

    public string ComponentName { get; set; }

    public string PId { get; set; }

    public int Sort { get; set; }

    public string Icon { get; set; }

    public int Type { get; set; }

    public bool Cache { get; set; }
    public bool Hidden { get; set; }

    public int SubCount { get; set; }

    public List<MenuVo> Children { get; set; }

    public bool Leaf { get; set; }
    public bool HasChildren { get; set; }

    public string Label { get; set; }

    public string CreateTime { get; set; }
    public string UpdateTime { get; set; }
}
