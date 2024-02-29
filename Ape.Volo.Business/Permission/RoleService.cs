using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Ape.Volo.Business.Base;
using Ape.Volo.Common.AttributeExt;
using Ape.Volo.Common.Exception;
using Ape.Volo.Common.Extention;
using Ape.Volo.Common.Global;
using Ape.Volo.Common.Model;
using Ape.Volo.Common.WebApp;
using Ape.Volo.Entity.Permission;
using Ape.Volo.IBusiness.Dto.Permission;
using Ape.Volo.IBusiness.ExportModel.Permission;
using Ape.Volo.IBusiness.Interface.Permission;
using Ape.Volo.IBusiness.QueryModel;
using SqlSugar;

namespace Ape.Volo.Business.Permission;

/// <summary>
/// 角色服务
/// </summary>
public class RoleService : BaseServices<Role>, IRoleService
{
    #region 字段

    #endregion

    #region 构造函数

    public RoleService(ApeContext apeContext) : base(apeContext)
    {
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
            var roleDepts = new List<RoleDepartment>();
            roleDepts.AddRange(createUpdateRoleDto.Depts.Select(rd => new RoleDepartment
                { RoleId = role.Id, DeptId = rd.Id }));
            await SugarClient.Insertable(roleDepts).ExecuteCommandAsync();
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
        await SugarClient.Deleteable<RoleDepartment>().Where(x => x.RoleId == role.Id).ExecuteCommandAsync();
        if (!createUpdateRoleDto.Depts.IsNullOrEmpty() && createUpdateRoleDto.Depts.Count > 0)
        {
            var roleDepts = new List<RoleDepartment>();
            roleDepts.AddRange(createUpdateRoleDto.Depts.Select(rd => new RoleDepartment
                { RoleId = role.Id, DeptId = rd.Id }));
            await SugarClient.Insertable(roleDepts).ExecuteCommandAsync();
        }

        return true;
    }

    [UseTran]
    public async Task<bool> DeleteAsync(HashSet<long> ids)
    {
        //返回用户列表的最大角色等级
        var roles = await TableWhere(x => ids.Contains(x.Id)).Includes(x => x.Users).ToListAsync();
        int userCount = 0;
        if (roles.Any(role => role.Users != null && role.Users.Count != 0))
        {
            userCount++;
        }

        if (userCount > 0)
        {
            throw new BadRequestException("存在用户关联，请解除后再试！");
        }


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
        Expression<Func<Role, List<Menu>>> navigationUserMenus = role => role.MenuList;
        Expression<Func<Role, List<Department>>> navigationUserDepts = role => role.DepartmentList;
        Expression<Func<Role, List<Apis>>> navigationRoleApis = role => role.Apis;

        var roleList =
            await SugarRepository.QueryPageListAsync<Role, Menu, Department, Apis>(whereExpression, pagination,
                null, null, navigationUserMenus,
                navigationUserDepts, navigationRoleApis);

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
        var roles = await TableWhere(whereExpression).Includes(x => x.DepartmentList).ToListAsync();
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

    public async Task<List<RoleDto>> QueryAllAsync()
    {
        var roleList = await TableWhere().Includes(x => x.MenuList).Includes(x => x.DepartmentList).ToListAsync();

        return ApeContext.Mapper.Map<List<RoleDto>>(roleList);
    }

    public async Task<int> QueryUserRoleLevelAsync(HashSet<long> ids)
    {
        List<int> levels = new List<int>();
        var roles =
            await SugarRepository.QueryMuchAsync<Role, UserRole, Role>(
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
            await SugarRepository.QueryMuchAsync<Role, UserRole, Role>(
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
        var role = await TableWhere(x => x.Id == createUpdateRoleDto.Id).Includes(x => x.Users).SingleAsync();
        await VerificationUserRoleLevelAsync(role.Level);


        //删除菜单
        List<RoleMenu> roleMenus = new List<RoleMenu>();
        if (!createUpdateRoleDto.Menus.IsNullOrEmpty() && createUpdateRoleDto.Menus.Count > 0)
        {
            roleMenus.AddRange(createUpdateRoleDto.Menus.Select(rm => new RoleMenu
                { RoleId = role.Id, MenuId = rm.Id }));

            await SugarClient.Deleteable<RoleMenu>().Where(x => x.RoleId == role.Id).ExecuteCommandAsync();
            await SugarClient.Insertable(roleMenus).ExecuteCommandAsync();
        }

        //删除用户缓存
        foreach (var user in role.Users)
        {
            await ApeContext.Cache.RemoveAsync(GlobalConstants.CacheKey.UserPermissionRoles +
                                               user.Id.ToString().ToMd5String16());
        }

        return true;
    }


    [UseTran]
    public async Task<bool> UpdateRolesApisAsync(CreateUpdateRoleDto createUpdateRoleDto)
    {
        var role = await TableWhere(x => x.Id == createUpdateRoleDto.Id).Includes(x => x.Users).SingleAsync();
        await VerificationUserRoleLevelAsync(role.Level);


        //删除菜单
        List<RoleApis> roleApis = new List<RoleApis>();
        if (createUpdateRoleDto.Apis.Any())
        {
            // 这里过滤一下自生成的一级节点ID
            createUpdateRoleDto.Apis = createUpdateRoleDto.Apis.Where(x => x.Id > 10000).ToList();
            roleApis.AddRange(createUpdateRoleDto.Apis.Select(ra => new RoleApis()
                { RoleId = role.Id, ApisId = ra.Id }));

            await SugarClient.Deleteable<RoleApis>().Where(x => x.RoleId == role.Id).ExecuteCommandAsync();
            await SugarClient.Insertable(roleApis).ExecuteCommandAsync();


            //删除用户缓存
            foreach (var user in role.Users)
            {
                await ApeContext.Cache.RemoveAsync(GlobalConstants.CacheKey.UserPermissionUrls +
                                                   user.Id.ToString().ToMd5String16());
            }
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
