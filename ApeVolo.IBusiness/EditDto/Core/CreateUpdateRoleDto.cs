using System.Collections.Generic;
using ApeVolo.Common.AttributeExt;
using ApeVolo.Entity.Do.Core;

namespace ApeVolo.IBusiness.EditDto.Core;

[AutoMapping(typeof(Role), typeof(CreateUpdateRoleDto))]
public class CreateUpdateRoleDto : BaseCreateUpdateEntityDto
{
    [ApeVoloRequired(Message = "角色名称不能为空！")]
    public string Name { get; set; }

    public int Level { get; set; }

    public string Description { get; set; }

    public string DataScope { get; set; }

    //[JsonIgnore] 
    [ApeVoloRequired(Message = "角色代码不能为空！")]
    public string Permission { get; set; }

    public List<CreateUpdateDepartmentDto> Depts { get; set; }

    public List<CreateUpdateMenuDto> Menus { get; set; }
}