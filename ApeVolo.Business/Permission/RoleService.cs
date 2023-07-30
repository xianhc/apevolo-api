using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ApeVolo.Business.Base;
using ApeVolo.Common.AttributeExt;
using ApeVolo.Common.Exception;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Global;
using ApeVolo.Common.Model;
using ApeVolo.Common.WebApp;
using ApeVolo.Entity.Permission;
using ApeVolo.IBusiness.Dto.Permission;
using ApeVolo.IBusiness.ExportModel.Permission;
using ApeVolo.IBusiness.Interface.Permission;
using ApeVolo.IBusiness.QueryModel;
using Castle.Core.Internal;
using SqlSugar;

namespace ApeVolo.Business.Permission;

/// <summary>
/// 角色服务
/// </summary>
public class RoleService : BaseServices<Role>, IRoleService
{
    #region 字段

    private readonly IMenuService _menuService;
    private readonly IRolesMenusService _rolesMenusService;
    private readonly IUserRolesService _userRolesService;
    private readonly IDepartmentService _departmentService;
    private readonly IRoleDeptService _roleDeptService;

    #endregion

    #region 构造函数

    public RoleService(IMenuService menuService, IDepartmentService departmentService
        , IRolesMenusService rolesMenusService, IUserRolesService userRolesService,
        IRoleDeptService roleDeptService, ApeContext apeContext) : base(apeContext)
    {
        _menuService = menuService;
        _departmentService = departmentService;
        _rolesMenusService = rolesMenusService;
        _userRolesService = userRolesService;
        _roleDeptService = roleDeptService;
    }

    #endregion

    #region 基础方法

    [UseTran]
    public async Task<bool> CreateAsync(CreateUpdateRoleDto createUpdateRoleDto)
    {
        await VerificationUserRoleLevelAsync(createUpdateRoleDto.Level);
        if (await TableWhere(r => r.Name == createUpdateRoleDto.Name).AnyAsync())
        {
            throw new BadRequestException($"角色名称=>{createUpdateRoleDto.Name}=>已存在!");
        }

        if (await TableWhere(r => r.Permission == createUpdateRoleDto.Permission).AnyAsync())
        {
            throw new BadRequestException($"权限标识=>{createUpdateRoleDto.Permission}=>已存在!");
        }

        var role = ApeContext.Mapper.Map<Role>(createUpdateRoleDto);
        await AddEntityAsync(role);

        if (!createUpdateRoleDto.Depts.IsNullOrEmpty() && createUpdateRoleDto.Depts.Count > 0)
        {
            var roleDepts = new List<RolesDepartments>();
            roleDepts.AddRange(createUpdateRoleDto.Depts.Select(rd => new RolesDepartments
                { RoleId = role.Id, DeptId = rd.Id }));
            await _roleDeptService.CreateAsync(roleDepts);
        }

        return true;
    }

    [UseTran]
    public async Task<bool> UpdateAsync(CreateUpdateRoleDto createUpdateRoleDto)
    {
        //取出待更新数据
        var oldRole = await TableWhere(x => x.Id == createUpdateRoleDto.Id).FirstAsync();
        if (oldRole.IsNull())
        {
            throw new BadRequestException("数据不存在！");
        }

        if (oldRole.Name != createUpdateRoleDto.Name &&
            await TableWhere(x => x.Name == createUpdateRoleDto.Name).AnyAsync())
        {
            throw new BadRequestException($"角色名称=>{createUpdateRoleDto.Name}=>已存在!");
        }

        if (oldRole.Permission != createUpdateRoleDto.Permission &&
            await TableWhere(x => x.Permission == createUpdateRoleDto.Permission).AnyAsync())
        {
            throw new BadRequestException($"权限标识=>{createUpdateRoleDto.Permission}=>已存在!");
        }

        await VerificationUserRoleLevelAsync(createUpdateRoleDto.Level);
        var role = ApeContext.Mapper.Map<Role>(createUpdateRoleDto);
        await UpdateEntityAsync(role);

        //删除部门权限关联
        await _roleDeptService.DeleteByRoleIdAsync(role.Id);
        if (!createUpdateRoleDto.Depts.IsNullOrEmpty() && createUpdateRoleDto.Depts.Count > 0)
        {
            var roleDepts = new List<RolesDepartments>();
            roleDepts.AddRange(createUpdateRoleDto.Depts.Select(rd => new RolesDepartments
                { RoleId = role.Id, DeptId = rd.Id }));
            await _roleDeptService.CreateAsync(roleDepts);
        }

        return true;
    }

    [UseTran]
    public async Task<bool> DeleteAsync(HashSet<long> ids)
    {
        //返回用户列表的最大角色等级
        var roles = await TableWhere(x => ids.Contains(x.Id)).ToListAsync();
        List<int> levels = new List<int>();
        levels.AddRange(roles.Select(x => x.Level).ToList());
        int minLevel = levels.Min();
        //验证当前用户角色等级是否大于待待删除的角色等级 ，不满足则抛异常
        await VerificationUserRoleLevelAsync(minLevel);

        //删除角色 角色部门 角色菜单
        return await LogicDelete<Role>(x => ids.Contains(x.Id)) > 0;
    }

    public async Task<List<RoleDto>> QueryAsync(RoleQueryCriteria roleQueryCriteria, Pagination pagination)
    {
        var whereExpression = GetWhereExpression(roleQueryCriteria);
        var roleList = await SugarRepository.QueryPageListAsync(whereExpression, pagination);
        foreach (var role in roleList)
        {
            //菜单
            var menus = ApeContext.Mapper.Map<List<Menu>>(
                await _menuService.FindByRoleIdAsync(role.Id));
            role.MenuList = menus;

            //部门
            var departments =
                ApeContext.Mapper.Map<List<Department>>(
                    await _departmentService.QueryByRoleIdAsync(role.Id));
            role.DepartmentList = departments;
        }

        return ApeContext.Mapper.Map<List<RoleDto>>(roleList);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="roleQueryCriteria"></param>
    /// <returns></returns>
    public async Task<List<ExportBase>> DownloadAsync(RoleQueryCriteria roleQueryCriteria)
    {
        var whereExpression = GetWhereExpression(roleQueryCriteria);
        var roles = await TableWhere(whereExpression).ToListAsync();
        foreach (var role in roles)
        {
            //菜单
            var menus = ApeContext.Mapper.Map<List<Menu>>(
                await _menuService.FindByRoleIdAsync(role.Id));
            role.MenuList = menus;

            //部门
            var departments =
                ApeContext.Mapper.Map<List<Department>>(
                    await _departmentService.QueryByRoleIdAsync(role.Id));
            role.DepartmentList = departments;
        }

        List<ExportBase> roleExports = new List<ExportBase>();
        roleExports.AddRange(roles.Select(x => new RoleExport()
        {
            Id = x.Id,
            Name = x.Name,
            Level = x.Level,
            Description = x.Description,
            DataScope = x.DataScope,
            DataDept = string.Join(",", x.DepartmentList.Select(d => d.Name).ToArray()),
            Permission = x.Permission,
            CreateTime = x.CreateTime
        }));
        return roleExports;
    }

    #endregion

    #region 扩展方法

    /// <summary>
    /// 获取用户全部角色
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<List<RoleSmallDto>> QueryByUserIdAsync(long id)
    {
        var roleSmallList =
            await SugarRepository.QueryMuchAsync<Role, UserRoles, Role>(
                (r, ur) => new object[]
                {
                    JoinType.Left, r.Id == ur.RoleId
                },
                (r, ur) => r,
                (r, ur) => ur.UserId == id
            );

        return ApeContext.Mapper.Map<List<RoleSmallDto>>(roleSmallList);
    }

    public async Task<List<RoleDto>> QueryAllAsync()
    {
        var roleList = await SugarRepository.QueryListAsync();
        foreach (var role in roleList)
        {
            //菜单
            var menus = ApeContext.Mapper.Map<List<Menu>>(
                await _menuService.FindByRoleIdAsync(role.Id));
            role.MenuList = menus;

            //部门
            var depts = ApeContext.Mapper.Map<List<Department>>(
                await _departmentService.QueryByRoleIdAsync(role.Id));
            role.DepartmentList = depts;
        }

        return ApeContext.Mapper.Map<List<RoleDto>>(roleList);
    }

    public async Task<int> QueryUserRoleLevelAsync(HashSet<long> ids)
    {
        List<int> levels = new List<int>();
        var roles =
            await SugarRepository.QueryMuchAsync<Role, UserRoles, Role>(
                (r, ur) => new object[]
                {
                    JoinType.Left, r.Id == ur.RoleId
                },
                (r, ur) => r,
                (r, ur) => ids.Contains(ur.UserId)
            );
        levels.AddRange(roles.Select(x => x.Level).ToList());
        int minLevel = levels.Min();
        return minLevel;
    }

    public async Task<int> VerificationUserRoleLevelAsync(int? level)
    {
        List<int> levels = new List<int>();
        var roles =
            await SugarRepository.QueryMuchAsync<Role, UserRoles, Role>(
                (r, ur) => new object[]
                {
                    JoinType.Left, r.Id == ur.RoleId
                },
                (r, ur) => r,
                (r, ur) => ur.UserId == ApeContext.LoginUserInfo.UserId
            );
        levels.AddRange(roles.Select(x => x.Level).ToList());
        int minLevel = levels.Min();
        if (level != null && level < minLevel)
        {
            throw new BadRequestException("您无权修改或删除比你角色等级更高的数据！");
        }

        return minLevel;
    }


    [UseTran]
    public async Task<bool> UpdateRolesMenusAsync(CreateUpdateRoleDto createUpdateRoleDto)
    {
        var role = await TableWhere(x => x.Id == createUpdateRoleDto.Id).SingleAsync();
        await VerificationUserRoleLevelAsync(role.Level);


        //删除菜单
        List<RoleMenu> roleMenus = new List<RoleMenu>();
        if (!createUpdateRoleDto.Menus.IsNullOrEmpty() && createUpdateRoleDto.Menus.Count > 0)
        {
            roleMenus.AddRange(createUpdateRoleDto.Menus.Select(rm => new RoleMenu
                { RoleId = role.Id, MenuId = rm.Id }));

            await _rolesMenusService.SugarClient.Deleteable<RoleMenu>(x => x.RoleId == role.Id).ExecuteCommandAsync();
            await _rolesMenusService.SugarClient.Insertable<RoleMenu>(roleMenus).ExecuteCommandAsync();
        }

        //获取所有用户  删除缓存
        var userRoles = await _userRolesService.QueryByRoleIdsAsync(new HashSet<long> { role.Id });
        foreach (var ur in userRoles)
        {
            await ApeContext.Cache.RemoveAsync(GlobalConstants.CacheKey.UserPermissionById +
                                               ur.UserId.ToString().ToMd5String16());
        }

        return true;
    }

    #endregion


    #region 条件表达式

    private static Expression<Func<Role, bool>> GetWhereExpression(RoleQueryCriteria roleQueryCriteria)
    {
        Expression<Func<Role, bool>> whereExpression = r => true;
        if (!roleQueryCriteria.RoleName.IsNullOrEmpty())
        {
            whereExpression = whereExpression.AndAlso(r =>
                r.Name.Contains(roleQueryCriteria.RoleName));
        }

        if (!roleQueryCriteria.CreateTime.IsNullOrEmpty())
        {
            whereExpression = whereExpression.AndAlso(r =>
                r.CreateTime >= roleQueryCriteria.CreateTime[0] && r.CreateTime <= roleQueryCriteria.CreateTime[1]);
        }

        return whereExpression;
    }

    #endregion
}
