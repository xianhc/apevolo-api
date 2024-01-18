using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Ape.Volo.Common.AttributeExt;
using Ape.Volo.IBusiness.Base;

namespace Ape.Volo.IBusiness.Dto.Permission;

[AutoMapping(typeof(ApeVolo.Entity.Permission.Role), typeof(CreateUpdateRoleDto))]
public class CreateUpdateRoleDto : BaseEntityDto<long>
{
    [Required]
    public string Name { get; set; }

    public int Level { get; set; }

    public string Description { get; set; }

    public string DataScope { get; set; }

    [Required]
    public string Permission { get; set; }

    public List<RoleDeptDto> Depts { get; set; }

    public List<RoleMenuDto> Menus { get; set; }
}
