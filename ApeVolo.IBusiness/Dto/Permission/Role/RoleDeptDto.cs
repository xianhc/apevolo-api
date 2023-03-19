using ApeVolo.Common.AttributeExt;
using ApeVolo.IBusiness.Base;

namespace ApeVolo.IBusiness.Dto.Permission.Role;

[AutoMapping(typeof(Entity.Permission.Department), typeof(RoleDeptDto))]
public class RoleDeptDto : RootId<long>
{
}