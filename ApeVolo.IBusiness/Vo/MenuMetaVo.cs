
namespace ApeVolo.IBusiness.Vo
{
    public class MenuMetaVO
    {
        public MenuMetaVO() { }
        public MenuMetaVO(string title, string icon, bool noCache)
        {
            this.Title = title;
            this.Icon = icon;
            this.NoCache = noCache;
        }
        public string Title { get; set; }

        public string Icon { get; set; }

        public bool NoCache { get; set; }
    }
}
