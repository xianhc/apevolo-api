using ApeVolo.Common.AttributeExt;
using ApeVolo.Entity.Do.Core;

namespace ApeVolo.IBusiness.EditDto.Core
{
    [AutoMapping(typeof(Menu), typeof(CreateUpdateMenuDto))]
    public class CreateUpdateMenuDto : BaseCreateUpdateEntityDto
    {
        [ApeVoloRequired(Message = "菜单标题不能为空！")]
        public string Title { get; set; }

        public string LinkUrl { get; set; }

        public string Path { get; set; }

        public string Permission { get; set; }

        public bool IFrame { get; set; }

        public string Component { get; set; }

        public string ComponentName { get; set; }

        public string PId { get; set; }

        public int MenuSort { get; set; }

        public string Icon { get; set; }

        [ApeVoloRequired(Message = "菜单类型不能为空！")]
        public int Type { get; set; }

        public bool Cache { get; set; }
        
        public bool Hidden { get; set; }
        
        public int SubCount { get; set; }
    }
}