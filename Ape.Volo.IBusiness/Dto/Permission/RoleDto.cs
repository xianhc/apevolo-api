using System.Collections.Generic;
using Ape.Volo.Common.AttributeExt;
using Ape.Volo.Entity.Permission;
using Ape.Volo.IBusiness.Base;
using Newtonsoft.Json;

namespace Ape.Volo.IBusiness.Dto.Permission;

[AutoMapping(typeof(Role), typeof(RoleDto))]
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
