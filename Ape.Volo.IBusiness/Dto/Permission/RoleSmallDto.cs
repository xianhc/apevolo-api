using Ape.Volo.Common.AttributeExt;
using Ape.Volo.Entity.Permission;

namespace Ape.Volo.IBusiness.Dto.Permission;

[AutoMapping(typeof(Role), typeof(RoleSmallDto))]
public class RoleSmallDto
{
    public long Id { get; set; }

    public string Name { get; set; }

    public string Permission { get; set; }

    public int Level { get; set; }

    public string DataScope { get; set; }
}
