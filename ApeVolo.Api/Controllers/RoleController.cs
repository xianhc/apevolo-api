using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Model;
using ApeVolo.IBusiness.Interface.Core;
using ApeVolo.IBusiness.Dto.Core;
using ApeVolo.IBusiness.EditDto.Core;
using ApeVolo.IBusiness.QueryModel;
using Microsoft.AspNetCore.Mvc;
using ApeVolo.Api.Controllers.Base;
using ApeVolo.Common.AttributeExt;
using ApeVolo.Common.Helper.Excel;
using Microsoft.AspNetCore.StaticFiles;
using System.IO;
using ApeVolo.Api.ActionExtension.Json;
using ApeVolo.Common.Helper;

namespace ApeVolo.Api.Controllers
{
    /// <summary>
    /// 角色管理
    /// </summary>
    [Area("Role")]
    [Route("/api/role")]
    public class RoleController : BaseApiController
    {
        #region 字段

        private readonly IRoleService _roleService;
        private readonly IUserRolesService _userRolesService;

        #endregion

        #region 构造函数

        public RoleController(IRoleService roleService, IUserRolesService userRolesService)
        {
            _roleService = roleService;
            _userRolesService = userRolesService;
        }

        #endregion

        #region 内部接口

        /// <summary>
        /// 添加角色
        /// </summary>
        /// <param name="createUpdateRoleDto"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("create")]
        [Description("添加角色")]
        public async Task<ActionResult<object>> Create(CreateUpdateRoleDto createUpdateRoleDto)
        {
            RequiredHelper.IsValid(createUpdateRoleDto);
            await _roleService.CreateAsync(createUpdateRoleDto);
            return Create("添加成功");
        }

        /// <summary>
        /// 更新角色
        /// </summary>
        /// <param name="createUpdateRoleDto"></param>
        /// <returns></returns>
        [HttpPut]
        [Description("更新角色")]
        [Route("edit")]
        public async Task<ActionResult<object>> Update([FromBody] CreateUpdateRoleDto createUpdateRoleDto)
        {
            RequiredHelper.IsValid(createUpdateRoleDto);
            await _roleService.UpdateAsync(createUpdateRoleDto);
            return NoContent();
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpDelete]
        [Description("删除角色")]
        [Route("delete")]
        [NoJsonParamter]
        public async Task<ActionResult<object>> Delete([FromBody] HashSet<string> ids)
        {
            if (ids == null || ids.Count < 0)
            {
                return Error("ids is null");
            }

            //检查待删除的角色是否有用户存在
            var userRoles = await _userRolesService.QueryByRoleIdsAsync(ids);
            if (!userRoles.IsNullOrEmpty() && userRoles.Count > 0)
            {
                return Error("所选角色存在用户关联，请解除关联再试！");
            }

            await _roleService.DeleteAsync(ids);
            return Success();
        }

        /// <summary>
        /// 查看单一角色
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        [Description("查看单一角色")]
        [ApeVoloAuthorize(new[] {"roles_list"})]
        public async Task<ActionResult<object>> QuerySingle(string id)
        {
            var role = await _roleService.QuerySingleAsync(id);
            return role.ToJson();
        }

        /// <summary>
        /// 查看角色列表
        /// </summary>
        /// <param name="roleQueryCriteria"></param>
        /// <param name="pagination"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("query")]
        [Description("获取角色列表")]
        public async Task<ActionResult<object>> Query(RoleQueryCriteria roleQueryCriteria,
            Pagination pagination)
        {
            var roleList = await _roleService.QueryAsync(roleQueryCriteria, pagination);

            return new ActionResultVm<RoleDto>()
            {
                Content = roleList,
                TotalElements = pagination.TotalElements
            }.ToJson();
        }

        /// <summary>
        /// 导出角色列表
        /// </summary>
        /// <param name="roleQueryCriteria"></param>
        /// <returns></returns>
        [HttpGet]
        [Description("导出角色列表")]
        [Route("download")]
        public async Task<ActionResult<object>> Download(RoleQueryCriteria roleQueryCriteria)
        {
            var exportRowModels = await _roleService.DownloadAsync(roleQueryCriteria);

            var filepath = ExcelHelper.ExportData(exportRowModels, "角色列表");

            var provider = new FileExtensionContentTypeProvider();
            FileInfo fileInfo = new FileInfo(filepath);
            var ext = fileInfo.Extension;
            new FileExtensionContentTypeProvider().Mappings.TryGetValue(ext, out var contently);
            return File(await System.IO.File.ReadAllBytesAsync(filepath), contently ?? "application/octet-stream",
                fileInfo.Name);
        }

        /// <summary>
        /// 获取全部角色
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("all")]
        [Description("获取全部角色")]
        [ApeVoloAuthorize(new[] {"roles_list"})]
        public async Task<ActionResult<object>> GetAllRoles()
        {
            var allRoles = await _roleService.QueryAllAsync();

            return allRoles.ToJson();
        }

        /// <summary>
        /// 获取当前用户角色等级
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("level")]
        [Description("获取当前用户角色等级")]
        [ApeVoloAuthorize(new[] {"roles_list"})]
        public async Task<ActionResult<object>> GetRoleLevel(int? level)
        {
            var curLevel = await _roleService.VerificationUserRoleLevelAsync(level);

            Dictionary<string, int> keyValuePairs = new Dictionary<string, int> {{"level", curLevel}};
            return keyValuePairs.ToJson();
        }

        #endregion
    }
}