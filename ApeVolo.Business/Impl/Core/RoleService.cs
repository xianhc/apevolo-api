using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ApeVolo.Business.Base;
using ApeVolo.Common.AttributeExt;
using ApeVolo.Common.Caches.Redis.Service;
using ApeVolo.Common.Exception;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Global;
using ApeVolo.Common.Helper.Excel;
using ApeVolo.Common.Model;
using ApeVolo.Common.WebApp;
using ApeVolo.Entity.Do.Core;
using ApeVolo.IBusiness.Dto.Core;
using ApeVolo.IBusiness.EditDto.Core;
using ApeVolo.IBusiness.Interface.Core;
using ApeVolo.IBusiness.QueryModel;
using ApeVolo.IRepository.Core;
using AutoMapper;
using Castle.Core.Internal;
using SqlSugar;

namespace ApeVolo.Business.Impl.Core;

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
    private readonly ISqlSugarClient _db;
    private readonly IRedisCacheService _redisCacheService;

    #endregion

    #region 构造函数

    public RoleService(IMapper mapper, IRoleRepository roleRepository, ICurrentUser currentUser,
        IMenuService menuService, IDepartmentService departmentService, ISqlSugarClient sqlSugarClient
        , IRolesMenusService rolesMenusService, IUserRolesService userRolesService,
        IRoleDeptService roleDeptService, IRedisCacheService redisCacheService)
    {
        _mapper = mapper;
        _baseDal = roleRepository;
        _currentUser = currentUser;
        _db = sqlSugarClient;
        _menuService = menuService;
        _departmentService = departmentService;
        _rolesMenusService = rolesMenusService;
        _userRolesService = userRolesService;
        _roleDeptService = roleDeptService;
        _redisCacheService = redisCacheService;
    }

    #endregion

    #region 基础方法

    [UseTran]
    public async Task<bool> CreateAsync(CreateUpdateRoleDto createUpdateRoleDto)
    {
        await VerificationUserRoleLevelAsync(createUpdateRoleDto.Level);
        if (await IsExistAsync(r => r.IsDeleted == false
                                    && r.Name == createUpdateRoleDto.Name))
        {
            throw new BadRequestException($"角色=>{createUpdateRoleDto.Name}=>已存在!");
        }

        if (await IsExistAsync(r => r.IsDeleted == false
                                    && r.Permission == createUpdateRoleDto.Permission))
        {
            throw new BadRequestException($"角色代码=>{createUpdateRoleDto.Permission}=>已存在!");
        }

        var role = _mapper.Map<Role>(createUpdateRoleDto);
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
        var oldRole = await QueryFirstAsync(x => x.IsDeleted == false && x.Id == createUpdateRoleDto.Id);
        if (oldRole.IsNull())
        {
            throw new BadRequestException("更新失败=》待更新数据不存在！");
        }

        if (oldRole.Name != createUpdateRoleDto.Name && await IsExistAsync(x => x.IsDeleted == false
                && x.Name == createUpdateRoleDto.Name))
        {
            throw new BadRequestException($"角色名称=>{createUpdateRoleDto.Name}=>已存在！");
        }

        if (oldRole.Permission != createUpdateRoleDto.Permission && await IsExistAsync(x =>
                x.IsDeleted == false
                && x.Permission == createUpdateRoleDto.Permission))
        {
            throw new BadRequestException($"角色代码=>{createUpdateRoleDto.Permission}=>已存在！");
        }

        await VerificationUserRoleLevelAsync(createUpdateRoleDto.Level);
        var role = _mapper.Map<Role>(createUpdateRoleDto);
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
        var roles = await QueryByIdsAsync(ids);
        List<int> levels = new List<int>();
        levels.AddRange(roles.Select(x => x.Level).ToList());
        int minLevel = levels.Min();
        //验证当前用户角色等级是否大于待待删除的角色等级 ，不满足则抛异常
        await VerificationUserRoleLevelAsync(minLevel);

        //删除角色 角色部门 角色菜单
        await DeleteEntityListAsync(roles);
        return true;
    }

    public async Task<List<RoleDto>> QueryAsync(RoleQueryCriteria roleQueryCriteria, Pagination pagination)
    {
        Expression<Func<Role, bool>> whereLambda = r => r.IsDeleted == false;
        if (!roleQueryCriteria.RoleName.IsNullOrEmpty())
        {
            whereLambda = whereLambda.And(r =>
                r.Name.Contains(roleQueryCriteria.RoleName));
        }

        if (!roleQueryCriteria.CreateTime.IsNullOrEmpty())
        {
            whereLambda = whereLambda.And(r =>
                r.CreateTime >= roleQueryCriteria.CreateTime[0] && r.CreateTime <= roleQueryCriteria.CreateTime[1]);
        }

        var roleList = await _baseDal.QueryMapperPageListAsync(async (it, cache) =>
        {
            //菜单
            var menus = _mapper.Map<List<Menu>>(await _menuService.FindByRoleIdAsync(it.Id));

            //部门
            var depts = _mapper.Map<List<Department>>(await _departmentService.QueryByRoleIdAsync(it.Id));

            it.MenuList = menus;
            it.DepartmentList = depts;
        }, whereLambda, pagination);

        return _mapper.Map<List<RoleDto>>(roleList);
    }


    public async Task<List<ExportRowModel>> DownloadAsync(RoleQueryCriteria roleQueryCriteria)
    {
        var roles = await QueryAsync(roleQueryCriteria, new Pagination { PageSize = 9999 });
        List<ExportRowModel> exportRowModels = new List<ExportRowModel>();
        List<ExportColumnModel> exportColumnModels;
        int point;
        roles.ForEach(role =>
        {
            point = 0;
            exportColumnModels = new List<ExportColumnModel>
            {
                new() { Key = "角色名称", Value = role.Name, Point = point++ },
                new() { Key = "等级", Value = role.Level.ToString(), Point = point++ },
                new() { Key = "描述", Value = role.Description, Point = point++ },
                new() { Key = "权限数据", Value = role.DataScope, Point = point++ },
                new()
                {
                    Key = "权限部门",
                    Value = string.Join(",", role.DepartmentList.Select(x => x.Name).ToArray()),
                    Point = point++
                },
                new() { Key = "权限代码", Value = role.Permission, Point = point++ },
                new()
                {
                    Key = "创建时间", Value = role.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"), Point = point++
                }
            };
            exportRowModels.Add(new ExportRowModel { exportColumnModels = exportColumnModels });
        });
        return exportRowModels;
    }

    #endregion

    #region 扩展方法

    public async Task<List<RoleDto>> QuerySingleAsync(long roleId)
    {
        var roleList = await _baseDal.QueryMapperAsync(async (it, cache) =>
        {
            //性能差 待优化

            //菜单
            var menus = _mapper.Map<List<Menu>>(await _menuService.FindByRoleIdAsync(it.Id));

            //部门
            var depts = _mapper.Map<List<Department>>(await _departmentService.QueryByRoleIdAsync(it.Id));

            it.MenuList = menus;
            it.DepartmentList = depts;
        }, r => r.IsDeleted == false);

        return _mapper.Map<List<RoleDto>>(roleList);
    }


    /// <summary>
    /// 获取用户全部角色
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<List<RoleSmallDto>> QueryByUserIdAsync(long userId)
    {
        var roleSmallList = await _baseDal.QueryMuchAsync<Role, UserRoles, Role>(
            (r, ur) => new object[]
            {
                JoinType.Left, r.Id == ur.RoleId
            },
            (r, ur) => r,
            (r, ur) => r.IsDeleted == false && ur.UserId == userId
        );

        return _mapper.Map<List<RoleSmallDto>>(roleSmallList);
    }

    public async Task<List<RoleDto>> QueryAllAsync()
    {
        var roleList = await _db.Queryable<Role>().Mapper(
            async (it, cache) =>
            {
                //菜单
                var menus = _mapper.Map<List<Menu>>(await _menuService.FindByRoleIdAsync(it.Id));

                //部门
                var depts = _mapper.Map<List<Department>>(await _departmentService.QueryByRoleIdAsync(it.Id));

                it.MenuList = menus;
                it.DepartmentList = depts;
            }
        ).ToListAsync();
        return _mapper.Map<List<RoleDto>>(roleList);
    }

    public async Task<int> QueryUserRoleLevelAsync(HashSet<long> ids)
    {
        List<int> levels = new List<int>();
        var roles = await _baseDal.QueryMuchAsync<Role, UserRoles, Role>(
            (r, ur) => new object[]
            {
                JoinType.Left, r.Id == ur.RoleId
            },
            (r, ur) => r,
            (r, ur) => r.IsDeleted == false && ids.Contains(ur.UserId)
        );
        levels.AddRange(roles.Select(x => x.Level).ToList());
        int minLevel = levels.Min();
        return minLevel;
    }

    public async Task<int> VerificationUserRoleLevelAsync(int? level)
    {
        List<int> levels = new List<int>();
        var roles = await _baseDal.QueryMuchAsync<Role, UserRoles, Role>(
            (r, ur) => new object[]
            {
                JoinType.Left, r.Id == ur.RoleId
            },
            (r, ur) => r,
            (r, ur) => r.IsDeleted == false && ur.UserId == _currentUser.Id //"737368938475687938"//
        );
        levels.AddRange(roles.Select(x => x.Level).ToList());
        int minLevel = levels.Min();
        if (level != null)
        {
            if (level < minLevel)
            {
                throw new BadRequestException("您无权修改或删除比你角色等级更高的数据！");
            }
        }

        return minLevel;
    }


    [UseTran]
    public async Task<bool> UpdateRolesMenusAsync(CreateUpdateRoleDto createUpdateRoleDto)
    {
        var role = await base.QuerySingleAsync(createUpdateRoleDto.Id);
        await VerificationUserRoleLevelAsync(role.Level);


        //删除菜单病删除
        List<RoleMenu> roleMenus = new List<RoleMenu>();
        if (!createUpdateRoleDto.Menus.IsNullOrEmpty() && createUpdateRoleDto.Menus.Count > 0)
        {
            roleMenus.AddRange(createUpdateRoleDto.Menus.Select(rm => new RoleMenu
                { RoleId = role.Id, MenuId = rm.Id }));

            await _rolesMenusService.DeleteAsync(new List<long> { role.Id });
            await _rolesMenusService.CreateAsync(roleMenus);
        }

        //获取所有用户  删除缓存
        var userRoles = await _userRolesService.QueryByRoleIdsAsync(new HashSet<long> { role.Id });
        userRoles.ForEach(async ur =>
        {
            await _redisCacheService.RemoveAsync(RedisKey.UserPermissionById +
                                                 ur.UserId.ToString().ToMd5String16());
        });
        return true;
    }

    #endregion
}