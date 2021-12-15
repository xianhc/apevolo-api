using System.Collections.Generic;
using System.Threading.Tasks;
using ApeVolo.Business.Base;
using ApeVolo.Common.AttributeExt;
using ApeVolo.Common.Global;
using ApeVolo.Entity.Do.Core;
using ApeVolo.IBusiness.Interface.Core;
using ApeVolo.IBusiness.Vo;
using ApeVolo.IRepository.Core;
using SqlSugar;

namespace ApeVolo.Business.Impl.Core
{
    public class PermissionService : BaseServices<Role>, IPermissionService
    {
        #region 字段

        #endregion

        #region 构造函数

        public PermissionService(IRoleRepository roleRepository)
        {
            _baseDal = roleRepository;
        }

        #endregion

        #region 基础方法

        /// <summary>
        /// 获取用户权限
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [RedisCaching(Expiration = 30, KeyPrefix = RedisKey.UserPermissionById)]
        public async Task<List<PermissionVO>> QueryUserPermissionAsync(long userId)
        {
            var permissionLists = await _baseDal.QueryMuchAsync<Menu, RoleMenu, UserRoles, PermissionVO>(
                (m, rm, ur) => new object[]
                {
                    JoinType.Left, m.Id == rm.MenuId,
                    JoinType.Left, rm.RoleId == ur.RoleId
                },
                (m, rm, ur) => new PermissionVO
                {
                    LinkUrl = m.LinkUrl,
                    Permission = m.Permission
                },
                (m, rm, ur) => m.IsDeleted == false && ur.UserId == userId
                , (m, rm, ur) => new {m.LinkUrl, m.Permission}
            );
            return permissionLists;
        }

        #endregion
    }
}