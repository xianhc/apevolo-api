using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ApeVolo.Common.AttributeExt;
using ApeVolo.IBusiness.Base;
using Newtonsoft.Json;

namespace ApeVolo.IBusiness.Dto.Permission;

[AutoMapping(typeof(Entity.Permission.Role), typeof(RoleDto))]
public class RoleDto : BaseEntityDto<long>
{
    [Display(Name = "Role.Name")]
    public string Name { get; set; }

    [Display(Name = "Role.Level")]
    public int Level { get; set; }

    [Display(Name = "Role.Description")]
    public string Description { get; set; }

    [Display(Name = "Role.DataScope")]
    public string DataScope { get; set; }

    [Display(Name = "Role.Permission")]
    public string Permission { get; set; }

    [JsonProperty(PropertyName = "menus")]
    public List<MenuDto> MenuList { get; set; }

    [JsonProperty(PropertyName = "depts")]
    public List<DepartmentDto> DepartmentList { get; set; }
}
