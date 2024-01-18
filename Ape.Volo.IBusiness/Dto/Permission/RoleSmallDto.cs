using Ape.Volo.Common.AttributeExt;

namespace Ape.Volo.IBusiness.Dto.Permission;

[AutoMapping(typeof(ApeVolo.Entity.Permission.Role), typeof(RoleSmallDto))]
public class RoleSmallDto
{
    public long Id { get; set; }

    public string Name { get; set; }

    public string Permission { get; set; }

    public int Level { get; set; }

    public string DataScope { get; set; }
}
