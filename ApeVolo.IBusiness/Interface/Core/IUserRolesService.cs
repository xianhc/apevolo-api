using ApeVolo.IBusiness.Base;
using ApeVolo.IBusiness.EditDto.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApeVolo.Entity.Do.Core;

namespace ApeVolo.IBusiness.Interface.Core
{
    /// <summary>
    /// 用户角色接口
    /// </summary>
    public interface IUserRolesService : IBaseServices<UserRoles>
    {
        #region 基础接口
        Task<int> CreateAsync(List<CreateUpdateUserRolesDto> createUpdateUserRoleDtos);
        Task<bool> DeleteByUserIdAsync(string userId);
        Task<List<UserRoles>> QueryAsync(string userId);
        Task<List<UserRoles>> QueryByRoleIdsAsync(HashSet<string> roleIds);
        #endregion
    }
}