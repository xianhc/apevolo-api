using ApeVolo.Common.AttributeExt;

namespace ApeVolo.IBusiness.Dto.Permission;

[AutoMapping(typeof(Entity.Permission.Role), typeof(RoleSmallDto))]
public class RoleSmallDto
{
    public long Id { get; set; }

    public string Name { get; set; }

    public string Permission { get; set; }

    public int Level { get; set; }

    public string DataScope { get; set; }
}
