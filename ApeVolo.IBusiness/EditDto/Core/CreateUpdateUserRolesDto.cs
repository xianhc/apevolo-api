using ApeVolo.Common.AttributeExt;
using ApeVolo.Entity.Do.Core;

namespace ApeVolo.IBusiness.EditDto.Core
{
    [AutoMapping(typeof(UserRoles), typeof(CreateUpdateUserRolesDto))]
    public class CreateUpdateUserRolesDto
    {
        public CreateUpdateUserRolesDto(string userId, string roleId)
        {
            UserId = userId;
            RoleId = roleId;
        }
        public string UserId { get; set; }

        public string RoleId { get; set; }
    }
}
