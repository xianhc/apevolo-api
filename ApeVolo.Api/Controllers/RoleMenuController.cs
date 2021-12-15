using System.ComponentModel;
using System.Threading.Tasks;
using ApeVolo.Api.Controllers.Base;
using ApeVolo.Common.AttributeExt;
using ApeVolo.IBusiness.EditDto.Core;
using ApeVolo.IBusiness.Interface.Core;
using Microsoft.AspNetCore.Mvc;

namespace ApeVolo.Api.Controllers
{
    /// <summary>
    /// 角色与菜单管理
    /// </summary>
    [Area("RoleMenu")]
    [Route("/api/")]
    public class RoleMenuController : BaseApiController
    {
        #region 字段

        private readonly IRoleService _roleService;

        #endregion

        #region 构造函数

        public RoleMenuController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        #endregion

        #region 对内接口

        /// <summary>
        /// 更新角色菜单关联
        /// </summary>
        /// <param name="createUpdateRoleDto"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("role/menu/edit")]
        [Description("更新角色菜单关联")]
        [ApeVoloAuthorize(new[] {"admin", "roles_edit"})]
        public async Task<ActionResult<object>> UpdateRolesMenus(CreateUpdateRoleDto createUpdateRoleDto)
        {
            await _roleService.UpdateRolesMenusAsync(createUpdateRoleDto);
            return NoContent();
        }

        #endregion
    }
}