using System.Collections.Generic;
using ApeVolo.Common.AttributeExt;
using ApeVolo.Entity.Do.Core;
using Newtonsoft.Json;

namespace ApeVolo.IBusiness.Dto.Core
{
    [AutoMapping(typeof(Role), typeof(RoleDto))]
    public class RoleDto : BaseEntityDto
    {
        public string Name { get; set; }
        public int Level { get; set; }

        public string Description { get; set; }

        public string DataScope { get; set; }

        // [JsonIgnore]
        public string Permission { get; set; }

        [JsonProperty(PropertyName = "menus")] public List<MenuDto> MenuList { get; set; }
        [JsonProperty(PropertyName = "depts")] public List<DepartmentDto> DepartmentList { get; set; }
    }
}