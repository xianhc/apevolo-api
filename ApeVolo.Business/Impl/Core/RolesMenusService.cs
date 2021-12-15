using System.Collections.Generic;
using System.Threading.Tasks;
using ApeVolo.Business.Base;
using ApeVolo.Entity.Do.Core;
using ApeVolo.IBusiness.Interface.Core;
using ApeVolo.IRepository.Core;

namespace ApeVolo.Business.Impl.Core
{
    /// <summary>
    /// 角色菜单服务
    /// </summary>
    public class RolesMenusService : BaseServices<RoleMenu>, IRolesMenusService
    {
        #region 构造函数

        public RolesMenusService(IRolesMenusRepository rolesMenusRepository)
        {
            _baseDal = rolesMenusRepository;
        }

        #endregion

        #region 基础方法

        public async Task<bool> DeleteAsync(List<long> roleIds)
        {
            return await _baseDal.DeleteAsync(rm => roleIds.Contains(rm.RoleId)) > 0;
        }

        public async Task<bool> CreateAsync(List<RoleMenu> roleMenu)
        {
            return await _baseDal.AddReturnBoolAsync(roleMenu);
        }

        #endregion
    }
}