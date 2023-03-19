using ApeVolo.Common.AttributeExt;
using ApeVolo.IBusiness.Base;

namespace ApeVolo.IBusiness.Dto.Permission.Role;

[AutoMapping(typeof(Entity.Permission.Menu), typeof(RoleMenuDto))]
public class RoleMenuDto : RootId<long>
{
}