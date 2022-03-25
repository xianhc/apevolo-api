using ApeVolo.Common.AttributeExt;
using ApeVolo.Entity.Do.Core;

namespace ApeVolo.IBusiness.Dto.Core;

[AutoMapping(typeof(Role), typeof(RoleSmallDto))]
public class RoleSmallDto
{
    //public RoleSmallDto(Role r)
    //{
    //    Id = r.Id;
    //    RoleName = r.RoleName;
    //    Level = r.Level;
    //    DataScope = r.DataScope;
    //}
    public long Id { get; set; }

    public string Name { get; set; }

    public string Permission { get; set; }

    public int Level { get; set; }

    public string DataScope { get; set; }
}