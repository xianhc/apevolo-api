using ApeVolo.Common.AttributeExt;

namespace ApeVolo.IBusiness.Dto.Permission.Role;

[AutoMapping(typeof(Entity.Do.Core.Role), typeof(RoleSmallDto))]
public class RoleSmallDto
{
    public long Id { get; set; }

    public string Name { get; set; }

    public string Permission { get; set; }

    public int Level { get; set; }

    public string DataScope { get; set; }
}