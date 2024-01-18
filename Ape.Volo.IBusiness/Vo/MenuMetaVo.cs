namespace Ape.Volo.IBusiness.Vo;

public class MenuMetaVO
{
    public MenuMetaVO()
    {
    }

    public MenuMetaVO(string title, string icon, bool noCache)
    {
        Title = title;
        Icon = icon;
        NoCache = noCache;
    }

    public string Title { get; set; }

    public string Icon { get; set; }

    public bool NoCache { get; set; }
}
