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
using ApeVolo.Common.Helper;
using ApeVolo.Common.Model;
using ApeVolo.Common.Resources;
using ApeVolo.Common.WebApp;
using ApeVolo.Entity.Permission;
using ApeVolo.IBusiness.Dto.Permission;
using ApeVolo.IBusiness.ExportModel.Permission;
using ApeVolo.IBusiness.Interface.Permission;
using ApeVolo.IBusiness.QueryModel;
using ApeVolo.IBusiness.Vo;
using SqlSugar;

namespace ApeVolo.Business.Permission;

public class MenuService : BaseServices<Menu>, IMenuService
{
    #region 字段

    private readonly IUserRolesService _userRolesService;

    #endregion

    #region 构造函数

    public MenuService(IUserRolesService userRolesService, ApeContext apeContext) : base(apeContext)
    {
        _userRolesService = userRolesService;
    }

    #endregion

    #region 基础方法

    /// <summary>
    /// 新增菜单
    /// </summary>
    /// <param name="createUpdateMenuDto"></param>
    /// <returns></returns>
    [UseTran]
    public async Task<bool> CreateAsync(CreateUpdateMenuDto createUpdateMenuDto)
    {
        if (await TableWhere(m => m.Title == createUpdateMenuDto.Title).AnyAsync())
        {
            throw new BadRequestException(Localized.Get("{0}{1}IsExist", Localized.Get("Menu"),
                createUpdateMenuDto.Title));
        }

        if (await TableWhere(m => m.ComponentName == createUpdateMenuDto.ComponentName).AnyAsync())
        {
            throw new BadRequestException(Localized.Get("{0}{1}IsExist", Localized.Get("Menu"),
                createUpdateMenuDto.ComponentName));
        }

        if (createUpdateMenuDto.Type != (int)MenuType.Catalog)
        {
            if (createUpdateMenuDto.LinkUrl.IsNullOrEmpty())
            {
                throw new BadRequestException(Localized.Get("{0}required", "LinkUrl"));
            }

            if (createUpdateMenuDto.Permission.IsNullOrEmpty())
            {
                throw new BadRequestException(Localized.Get("{0}required", "Permission"));
            }
        }

        if (createUpdateMenuDto.IFrame)
        {
            string http = "http://", https = "https://";
            if (!(createUpdateMenuDto.Path.ToLower().StartsWith(http) ||
                  createUpdateMenuDto.Path.ToLower().StartsWith(https)))
            {
                throw new BadRequestException("外链菜单必须以http://或者https://开头");
            }
        }

        if (createUpdateMenuDto.ParentId.IsNotNull() && createUpdateMenuDto.ParentId == 0)
        {
            createUpdateMenuDto.ParentId = null;
        }

        var menu = ApeContext.Mapper.Map<Menu>(createUpdateMenuDto);

        await AddEntityAsync(menu);
        if (createUpdateMenuDto.ParentId.IsNotNull())
        {
            //清理缓存
            await ApeContext.RedisCache.RemoveAsync(GlobalConstants.CacheKey.LoadMenusByPId +
                                                    menu.ParentId.ToString().ToMd5String16());
            var tmpMenu = await SugarRepository.QueryFirstAsync(x => x.Id == menu.ParentId);
            if (tmpMenu.IsNotNull())
            {
                var tmpMenuList =
                    await SugarRepository.QueryListAsync(x => x.ParentId == tmpMenu.Id);
                tmpMenu.SubCount = tmpMenuList.Count;
                await UpdateEntityAsync(tmpMenu);
            }
        }

        return true;
    }

    [UseTran]
    public async Task<bool> UpdateAsync(CreateUpdateMenuDto createUpdateMenuDto)
    {
        //取出待更新数据
        var oldMenu = await TableWhere(x => x.Id == createUpdateMenuDto.Id).FirstAsync();
        if (oldMenu.IsNull())
        {
            throw new BadRequestException(Localized.Get("DataNotExist"));
        }

        if (oldMenu.Title != createUpdateMenuDto.Title &&
            await TableWhere(x => x.Title == createUpdateMenuDto.Title).AnyAsync())
        {
            throw new BadRequestException(Localized.Get("{0}{1}IsExist", Localized.Get("Menu"),
                createUpdateMenuDto.Title));
        }

        if (oldMenu.Permission != createUpdateMenuDto.Permission &&
            await TableWhere(x => x.Permission == createUpdateMenuDto.Permission).AnyAsync())
        {
            throw new BadRequestException(Localized.Get("{0}{1}IsExist", Localized.Get("Menu"),
                createUpdateMenuDto.Permission));
        }


        if (createUpdateMenuDto.IFrame)
        {
            string http = "http://", https = "https://";
            if (!(createUpdateMenuDto.Path.ToLower().StartsWith(http) ||
                  createUpdateMenuDto.Path.ToLower().StartsWith(https)))
            {
                throw new BadRequestException("外链菜单必须以http://或者https://开头");
            }
        }

        if (!createUpdateMenuDto.ComponentName.IsNullOrEmpty())
        {
            var menu1 = await SugarRepository.QueryFirstAsync(m =>
                m.ComponentName.Equals(createUpdateMenuDto.ComponentName));
            if (menu1 != null && menu1.Id != createUpdateMenuDto.Id)
            {
                throw new BadRequestException(Localized.Get("{0}{1}IsExist", Localized.Get("Menu"),
                    createUpdateMenuDto.ComponentName));
            }
        }

        if (createUpdateMenuDto.ParentId.IsNotNull() && createUpdateMenuDto.ParentId == 0)
        {
            createUpdateMenuDto.ParentId = null;
        }

        var menu2 = ApeContext.Mapper.Map<Menu>(createUpdateMenuDto);
        //清理缓存
        await ApeContext.RedisCache.RemoveAsync(GlobalConstants.CacheKey.LoadMenusById +
                                                menu2.Id.ToString().ToMd5String16());
        if (menu2.ParentId.IsNotNull())
        {
            await ApeContext.RedisCache.RemoveAsync(GlobalConstants.CacheKey.LoadMenusByPId +
                                                    menu2.ParentId.ToString().ToMd5String16());
        }

        await UpdateEntityAsync(menu2);
        //重新计算子节点个数
        if (oldMenu.ParentId != menu2.ParentId)
        {
            if (menu2.ParentId.IsNotNull())
            {
                var tmpMenu = await SugarRepository.QueryFirstAsync(x => x.Id == menu2.ParentId);
                if (tmpMenu.IsNotNull())
                {
                    var tmpMenuList =
                        await SugarRepository.QueryListAsync(x => x.ParentId == tmpMenu.Id);
                    tmpMenu.SubCount = tmpMenuList.Count;
                    await UpdateEntityAsync(tmpMenu);
                }

                if (oldMenu.ParentId.IsNotNull())
                {
                    var tmpMenu2 = await SugarRepository.QueryFirstAsync(x => x.Id == oldMenu.ParentId);
                    if (tmpMenu2.IsNotNull())
                    {
                        var tmpMenu2List =
                            await SugarRepository.QueryListAsync(x => x.ParentId == tmpMenu2.Id);
                        tmpMenu2.SubCount = tmpMenu2List.Count;
                        await UpdateEntityAsync(tmpMenu2);
                    }
                }
            }
        }

        return true;
    }

    public async Task<bool> DeleteAsync(HashSet<long> ids)
    {
        var idList = new List<long>();
        foreach (var id in ids)
        {
            if (!idList.Contains(id))
            {
                idList.Add(id);
            }

            var menus = await SugarRepository.QueryListAsync(m => m.ParentId == id);
            await FindChildIdsAsync(menus, idList);
        }

        var isTrue = await LogicDelete<Menu>(x => ids.Contains(x.Id)) > 0;
        if (isTrue)
        {
            //清除缓存
            foreach (var id in idList)
            {
                await ApeContext.RedisCache.RemoveAsync(GlobalConstants.CacheKey.LoadMenusById +
                                                        id.ToString().ToMd5String16());
                await ApeContext.RedisCache.RemoveAsync(GlobalConstants.CacheKey.LoadMenusByPId +
                                                        id.ToString().ToMd5String16());
            }
        }

        return isTrue;
    }

    public async Task<List<MenuDto>> QueryAsync(MenuQueryCriteria menuQueryCriteria, Pagination pagination)
    {
        var whereExpression = GetWhereExpression(menuQueryCriteria);
        pagination.SortFields = new List<string> { "sort asc" };
        var menus = await SugarRepository.QueryPageListAsync(whereExpression, pagination);
        var menuDtos = ApeContext.Mapper.Map<List<MenuDto>>(menus);
        return menuDtos;
    }


    public async Task<List<ExportBase>> DownloadAsync(MenuQueryCriteria menuQueryCriteria)
    {
        var whereExpression = GetWhereExpression(menuQueryCriteria);
        var menus = await TableWhere(whereExpression).ToListAsync();
        List<ExportBase> roleExports = new List<ExportBase>();
        roleExports.AddRange(menus.Select(x => new MenuExport()
        {
            Title = x.Title,
            LinkUrl = x.LinkUrl,
            Path = x.Path,
            Permission = x.Permission,
            IsFrame = x.IFrame ? BoolState.True : BoolState.False,
            Component = x.Component,
            ComponentName = x.ComponentName,
            PId = x.ParentId ?? 0,
            Sort = x.Sort,
            Icon = x.Icon,
            MenuType = GetMenuTypeName(x.Type),
            IsCache = x.Cache ? BoolState.True : BoolState.False,
            IsHidden = x.Hidden ? BoolState.True : BoolState.False,
            SubCount = x.SubCount,
            CreateTime = x.CreateTime
        }));
        return roleExports;
    }

    #endregion

    #region 扩展方法

    public async Task<List<MenuDto>> QueryAllAsync()
    {
        var menuDtos = await ApeContext.RedisCache.GetAsync<List<MenuDto>>("menus:LoadAllMenu");
        if (menuDtos.IsNotNull())
        {
            return menuDtos;
        }

        menuDtos = ApeContext.Mapper.Map<List<MenuDto>>(await SugarRepository.QueryListAsync());
        if (menuDtos.IsNotNull())
        {
            await ApeContext.RedisCache.SetAsync("menus:LoadAllMenu", menuDtos, TimeSpan.FromSeconds(120), null);
        }

        return menuDtos;
    }

    /// <summary>
    /// 构建前端路由菜单
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns></returns>
    [RedisCaching(KeyPrefix = GlobalConstants.CacheKey.UserBuildMenuById)]
    public async Task<List<MenuTreeVo>> BuildTreeAsync(long userId)
    {
        var userRoles = await _userRolesService.QueryAsync(userId);
        var roleIds = new List<long>();
        roleIds.AddRange(userRoles.Select(r => r.RoleId));
        var menuList = await SugarRepository.QueryMuchAsync<Menu, RoleMenu, MenuDto>(
            (m, rm) => new object[]
            {
                JoinType.Left, m.Id == rm.MenuId
            }, (m, rm) => new MenuDto
            {
                Title = m.Title,
                LinkUrl = m.LinkUrl,
                Path = m.Path,
                Permission = m.Permission,
                IFrame = m.IFrame,
                Component = m.Component,
                ComponentName = m.ComponentName,
                ParentId = m.ParentId,
                Sort = m.Sort,
                Icon = m.Icon,
                Type = m.Type,
                IsDeleted = m.IsDeleted,
                Id = m.Id,
                CreateTime = m.CreateTime,
                CreateBy = m.CreateBy
            },
            (m, rm) => roleIds.Contains(rm.RoleId) && m.Type != (int)MenuType.Button
            , (m, rm) => new
            {
                m.Title, m.LinkUrl, m.Path, m.Permission, m.IFrame, m.Component, m.ComponentName,
                PId = m.ParentId,
                MenuSort = m.Sort, m.Icon, m.Type, m.IsDeleted, m.Id, m.CreateBy, m.CreateTime
            },
            "sort asc");
        var menuListChild = TreeHelper<MenuDto>.ListToTrees(menuList, "Id", "ParentId", null);
        return await BuildAsync(menuListChild);
    }


    [RedisCaching(Expiration = 30, KeyPrefix = GlobalConstants.CacheKey.LoadMenusById)]
    public async Task<List<MenuDto>> FindSuperiorAsync(long id)
    {
        Expression<Func<Menu, bool>> whereLambda = m => true;
        var menu = await TableWhere(x => x.Id == id).SingleAsync();
        List<MenuDto> menuDtoList;
        if (menu.ParentId.IsNull())
        {
            var menus = await SugarRepository.QueryListAsync(x => x.ParentId == null, x => x.Sort,
                OrderByType.Asc);
            menuDtoList = ApeContext.Mapper.Map<List<MenuDto>>(menus);
            menuDtoList.ForEach(x => x.Children = null);
        }
        else
        {
            //查出同级菜单ID
            List<long> parentIds = new List<long>();
            parentIds.Add(Convert.ToInt64(menu.ParentId));
            await GetParentIdsAsync(menu, parentIds);
            whereLambda =
                whereLambda.AndAlso(m => parentIds.Contains(Convert.ToInt64(m.ParentId)) || m.ParentId == null);

            //可以优化语句
            var menus = await SugarRepository.QueryListAsync(whereLambda, x => x.Sort, OrderByType.Asc);
            var allMenu = await SugarRepository.QueryListAsync();
            foreach (var m in menus)
            {
                if (parentIds.Contains(m.Id) && m.ParentId.IsNull())
                {
                    m.Children = allMenu.Where(x => x.ParentId == m.Id).ToList();
                }
            }


            var tempDtos = ApeContext.Mapper.Map<List<MenuDto>>(menus);
            menuDtoList = TreeHelper<MenuDto>.ListToTrees(tempDtos, "Id", "ParentId", null);
            foreach (var item in menuDtoList)
            {
                if (item.Children.Count == 0)
                {
                    item.Children = null;
                }
                else
                {
                    foreach (var item2 in item.Children)
                    {
                        if (item2.Children.Count == 0)
                        {
                            item2.Children = null;
                        }
                        else
                        {
                            foreach (var item3 in item2.Children.Where(item3 => item3.Children.Count == 0))
                            {
                                item3.Children = null;
                            }
                        }
                    }
                }
            }
        }

        return menuDtoList;
    }

    /// <summary>
    /// 构建前端路由菜单
    /// </summary>
    /// <param name="menuDtOs"></param>
    /// <returns></returns>
    private async Task<List<MenuTreeVo>> BuildAsync(List<MenuDto> menuDtOs)
    {
        List<MenuTreeVo> menuVos = new List<MenuTreeVo>();
        MenuTreeVo menuVo = null;
        List<MenuDto> menuDtoList = null;

        foreach (var menu in menuDtOs)
        {
            menuDtoList = menu.Children;
            menuVo = new MenuTreeVo
            {
                Name = menu.ComponentName.IsNullOrEmpty() ? menu.Title : menu.ComponentName,
                Path = menu.ParentId.IsNull() ? "/" + menu.Path : menu.Path,
                Hidden = menu.Hidden
            };

            if (!menu.IFrame)
            {
                if (menu.ParentId.IsNull())
                {
                    menuVo.Component = menu.Component.IsNullOrEmpty() ? "Layout" : menu.Component;
                }
                else if (!menu.Component.IsNullOrEmpty())
                {
                    menuVo.Component = menu.Component;
                }
            }

            menuVo.Meta = new MenuMetaVO(menu.Title, menu.Icon, !menu.Cache);
            if (menuDtoList is { Count: > 0 })
            {
                menuVo.AlwaysShow = true;
                menuVo.Redirect = "noredirect";
                menuVo.Children = await BuildAsync(menuDtoList);
            }

            menuVos.Add(menuVo);
        }

        return menuVos;
    }

    [RedisCaching(Expiration = 30, KeyPrefix = GlobalConstants.CacheKey.LoadMenusByPId)]
    public async Task<List<MenuDto>> FindByPIdAsync(long pid = 0)
    {
        List<MenuDto> menuDtos = null;
        Expression<Func<Menu, bool>> whereLambda = m => true;
        whereLambda = pid == 0
            ? whereLambda.AndAlso(m => m.ParentId == null)
            : whereLambda.AndAlso(m => m.ParentId == pid);

        menuDtos = ApeContext.Mapper.Map<List<MenuDto>>(await SugarRepository.QueryListAsync(whereLambda,
            o => o.Sort,
            OrderByType.Asc));
        foreach (var item in menuDtos)
        {
            item.Children = null;
        }

        return menuDtos;
    }

    /// <summary>
    /// 根据角色获取菜单数据
    /// </summary>
    /// <param name="roleId"></param>
    /// <returns></returns>
    public async Task<List<MenuDto>> FindByRoleIdAsync(long roleId)
    {
        var menuList = await SugarRepository.QueryMuchAsync<Menu, RoleMenu, MenuDto>(
            (m, rm) => new object[]
            {
                JoinType.Left, m.Id == rm.MenuId
            }, (m, rm) => new MenuDto
            {
                Title = m.Title,
                LinkUrl = m.LinkUrl,
                Path = m.Path,
                Permission = m.Permission,
                IFrame = m.IFrame,
                Component = m.Component,
                ComponentName = m.ComponentName,
                ParentId = m.ParentId,
                Sort = m.Sort,
                Icon = m.Icon,
                Type = m.Type,
                IsDeleted = m.IsDeleted,
                Id = m.Id,
                CreateTime = m.CreateTime,
                CreateBy = m.CreateBy
            },
            (m, rm) => roleId == rm.RoleId
        );
        return menuList;
    }

    private async Task<List<long>> GetParentIdsAsync(Menu m, List<long> parentIds)
    {
        var menu = await SugarRepository.QueryFirstAsync(x => x.Id == m.ParentId);
        if (menu.IsNull() || menu.ParentId.IsNull())
        {
            //parentIds.Add(menu.PId);
            return await Task.FromResult(parentIds);
        }

        parentIds.Add(Convert.ToInt64(menu.ParentId));
        return await GetParentIdsAsync(menu, parentIds);
    }

    /// <summary>
    /// 获取所有下级菜单
    /// </summary>
    /// <param name="menuList"></param>
    /// <param name="ids"></param>
    /// <returns></returns>
    private async Task FindChildIdsAsync(List<Menu> menuList, List<long> ids)
    {
        if (menuList is { Count: > 0 })
        {
            foreach (var menu in menuList)
            {
                if (!ids.Contains(menu.Id))
                {
                    ids.Add(menu.Id);
                }

                List<Menu> menus = await SugarRepository.QueryListAsync(m => m.ParentId == menu.Id);
                if (menus is { Count: > 0 })
                {
                    await FindChildIdsAsync(menus, ids);
                }
            }
        }

        await Task.FromResult(ids);
    }

    public async Task<List<long>> FindChildAsync(long id)
    {
        List<long> ids = new List<long> { id };
        var menus = await QueryAllAsync();
        if (menus.Count > 0)
        {
            var menus2 = menus.Where(x => x.ParentId == id).ToList();
            if (menus2.Count > 0)
            {
                var ids2 = menus2.Select(x => x.Id).ToList();
                ids.AddRange(ids2);

                var menus3 = menus.Where(x => ids2.Contains(Convert.ToInt64(x.ParentId))).ToList();
                if (menus3.Count > 0)
                {
                    ids.AddRange(menus3.Select(x => x.Id).ToList());
                }
            }

            return ids;
        }

        return new List<long>();
    }

    #endregion

    #region 私有方法

    private static string GetMenuTypeName(int type)
    {
        var name = type switch
        {
            1 => "目录",
            2 => "菜单",
            3 => "按钮",
            _ => "未知"
        };

        return name;
    }

    #endregion

    #region 条件表达式

    private static Expression<Func<Menu, bool>> GetWhereExpression(MenuQueryCriteria menuQueryCriteria)
    {
        Expression<Func<Menu, bool>> whereExpression = m => true;
        if (!menuQueryCriteria.Title.IsNullOrEmpty())
        {
            whereExpression = whereExpression.AndAlso(m =>
                m.Title.Contains(menuQueryCriteria.Title));
        }

        whereExpression = menuQueryCriteria.ParentId.IsNull()
            ? whereExpression.AndAlso(m => m.ParentId == null)
            : whereExpression.AndAlso(m => m.ParentId == menuQueryCriteria.ParentId);

        return whereExpression;
    }

    #endregion
}
