using System.Collections.Generic;
using ApeVolo.Common.AttributeExt;
using ApeVolo.IBusiness.Base;
using Newtonsoft.Json;

namespace ApeVolo.IBusiness.Dto.Permission;

[AutoMapping(typeof(Entity.Permission.Role), typeof(RoleDto))]
public class RoleDto : BaseEntityDto<long>
{
    public string Name { get; set; }

    public int Level { get; set; }

    public string Description { get; set; }

    public string DataScope { get; set; }

    public string Permission { get; set; }

    [JsonProperty(PropertyName = "menus")]
    public List<MenuDto> MenuList { get; set; }

    [JsonProperty(PropertyName = "depts")]
    public List<DepartmentDto> DepartmentList { get; set; }
}
