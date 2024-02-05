namespace Ape.Volo.IBusiness.Vo;

/// <summary>
/// 菜单Meta
/// </summary>
public class MenuMetaVO
{
    /// <summary>
    /// 构造函数
    /// </summary>
    public MenuMetaVO()
    {
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="title"></param>
    /// <param name="icon"></param>
    /// <param name="noCache"></param>
    public MenuMetaVO(string title, string icon, bool noCache)
    {
        Title = title;
        Icon = icon;
        NoCache = noCache;
    }

    /// <summary>
    /// 标题
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Icon
    /// </summary>
    public string Icon { get; set; }

    /// <summary>
    /// 不缓存
    /// </summary>
    public bool NoCache { get; set; }
}
