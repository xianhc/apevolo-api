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
using ApeVolo.Common.Helper;
using ApeVolo.Common.Helper.Excel;
using ApeVolo.Common.Model;
using ApeVolo.Entity.Do.Core;
using ApeVolo.IBusiness.Dto.Core;
using ApeVolo.IBusiness.EditDto.Core;
using ApeVolo.IBusiness.Interface.Core;
using ApeVolo.IBusiness.QueryModel;
using ApeVolo.IBusiness.Vo;
using ApeVolo.IRepository.Core;
using AutoMapper;
using SqlSugar;

namespace ApeVolo.Business.Impl.Core;

public class MenuService : BaseServices<Menu>, IMenuService
{
    #region 字段

    private readonly IUserRolesService _userRolesService;
    private readonly IRedisCacheService _redisCacheService;

    #endregion

    #region 构造函数

    public MenuService(IMenuRepository menuRepository, IMapper mapper, IUserRolesService userRolesService,
        IRedisCacheService redisCacheService)
    {
        BaseDal = menuRepository;
        Mapper = mapper;
        _userRolesService = userRolesService;
        _redisCacheService = redisCacheService;
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
        if (await IsExistAsync(m => m.IsDeleted == false && m.Title == createUpdateMenuDto.Title))
        {
            throw new BadRequestException($"菜单名称=》{createUpdateMenuDto.Title}=》已存在");
        }

        if (await IsExistAsync(m => m.IsDeleted == false && m.ComponentName == createUpdateMenuDto.ComponentName))
        {
            throw new BadRequestException($"菜单组件=》{createUpdateMenuDto.ComponentName}=》已存在");
        }

        if (createUpdateMenuDto.Type != (int)MenuType.Catalog)
        {
            if (createUpdateMenuDto.LinkUrl.IsNullOrEmpty())
            {
                throw new BadRequestException("URL不能为空！");
            }

            if (createUpdateMenuDto.Permission.IsNullOrEmpty())
            {
                throw new BadRequestException("权限标识不能为空！");
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

        if (createUpdateMenuDto.PId.IsNotNull() && createUpdateMenuDto.PId == 0)
        {
            createUpdateMenuDto.PId = null;
        }

        var menu = Mapper.Map<Menu>(createUpdateMenuDto);

        await AddEntityAsync(menu);
        if (createUpdateMenuDto.PId.IsNotNull())
        {
            //清理缓存
            await _redisCacheService.RemoveAsync(RedisKey.LoadMenusByPId + menu.PId.ToString().ToMd5String16());
            var tmpMenu = await BaseDal.QueryFirstAsync(x => x.IsDeleted == false && x.Id == menu.PId);
            if (tmpMenu.IsNotNull())
            {
                var tmpMenuList =
                    await BaseDal.QueryListAsync(x => x.IsDeleted == false && x.PId == tmpMenu.Id);
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
        var oldMenu = await QueryFirstAsync(x => x.IsDeleted == false && x.Id == createUpdateMenuDto.Id);
        if (oldMenu.IsNull())
        {
            throw new BadRequestException("更新失败=》待更新数据不存在！");
        }

        if (oldMenu.Title != createUpdateMenuDto.Title && await IsExistAsync(x => x.IsDeleted == false
                && x.Title == createUpdateMenuDto.Title))
        {
            throw new BadRequestException($"菜单标题=>{createUpdateMenuDto.Title}=>已存在！");
        }

        if (oldMenu.Permission != createUpdateMenuDto.Permission && await IsExistAsync(x =>
                x.IsDeleted == false
                && x.Permission == createUpdateMenuDto.Permission))
        {
            throw new BadRequestException($"角色代码=>{createUpdateMenuDto.Permission}=>已存在！");
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

        var menu = await BaseDal.QueryFirstAsync(m =>
            m.IsDeleted == false && m.Title.Equals(createUpdateMenuDto.Title));
        if (menu != null && menu.Id != createUpdateMenuDto.Id)
        {
            throw new BadRequestException($"菜单名称=>{createUpdateMenuDto.Title}=>已存在");
        }

        if (!createUpdateMenuDto.ComponentName.IsNullOrEmpty())
        {
            var menu1 = await BaseDal.QueryFirstAsync(m =>
                m.IsDeleted == false && m.ComponentName.Equals(createUpdateMenuDto.ComponentName));
            if (menu1 != null && menu1.Id != createUpdateMenuDto.Id)
            {
                throw new BadRequestException($"菜单组件=>{createUpdateMenuDto.ComponentName}=>已存在");
            }
        }

        if (createUpdateMenuDto.PId.IsNotNull() && createUpdateMenuDto.PId == 0)
        {
            createUpdateMenuDto.PId = null;
        }

        var menu2 = Mapper.Map<Menu>(createUpdateMenuDto);
        //清理缓存
        await _redisCacheService.RemoveAsync(RedisKey.LoadMenusById + menu2.Id.ToString().ToMd5String16());
        if (menu2.PId.IsNotNull())
        {
            await _redisCacheService.RemoveAsync(RedisKey.LoadMenusByPId + menu2.PId.ToString().ToMd5String16());
        }

        await UpdateEntityAsync(menu2);
        //重新计算子节点个数
        if (oldMenu.PId != menu2.PId)
        {
            if (menu2.PId.IsNotNull())
            {
                var tmpMenu = await BaseDal.QueryFirstAsync(x => x.IsDeleted == false && x.Id == menu2.PId);
                if (tmpMenu.IsNotNull())
                {
                    var tmpMenuList =
                        await BaseDal.QueryListAsync(x => x.IsDeleted == false && x.PId == tmpMenu.Id);
                    tmpMenu.SubCount = tmpMenuList.Count;
                    await UpdateEntityAsync(tmpMenu);
                }

                if (oldMenu.PId.IsNotNull())
                {
                    var tmpMenu2 = await BaseDal.QueryFirstAsync(x => x.IsDeleted == false && x.Id == oldMenu.PId);
                    if (tmpMenu2.IsNotNull())
                    {
                        var tmpMenu2List =
                            await BaseDal.QueryListAsync(x => x.IsDeleted == false && x.PId == tmpMenu2.Id);
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
        ids.ForEach(async id =>
        {
            if (!idList.Contains(id))
            {
                idList.Add(id);
            }

            var menus = await BaseDal.QueryListAsync(m => m.IsDeleted == false && m.PId == id);
            await FindChildIdsAsync(menus, idList);
        });

        var menuList = await QueryByIdsAsync(idList);
        var isTrue = await DeleteEntityListAsync(menuList);
        if (isTrue)
        {
            //清除缓存
            idList.ForEach(async id =>
            {
                await _redisCacheService.RemoveAsync(RedisKey.LoadMenusById + id.ToString().ToMd5String16());
                await _redisCacheService.RemoveAsync(RedisKey.LoadMenusByPId + id.ToString().ToMd5String16());
            });
        }

        return isTrue;
    }

    public async Task<List<MenuDto>> QueryAsync(MenuQueryCriteria menuQueryCriteria, Pagination pagination)
    {
        Expression<Func<Menu, bool>> whereLambda = m => m.IsDeleted == false;
        if (!menuQueryCriteria.Title.IsNullOrEmpty())
        {
            whereLambda = whereLambda.And(m =>
                m.Title.Contains(menuQueryCriteria.Title));
        }

        whereLambda = menuQueryCriteria.PId.IsNull()
            ? whereLambda.And(m => m.PId == null)
            : whereLambda.And(m => m.PId == menuQueryCriteria.PId);

        pagination.SortFields = new List<string> { "menu_sort asc" };
        var menus = await BaseDal.QueryMapperPageListAsync(async (it, cache) =>
        {
            var childrenList = await cache.Get(list =>
            {
                var ids = list.Select(i => i.Id).ToList();
                return BaseDal.QueryListAsync(m => m.IsDeleted == false && ids.Contains(Convert.ToInt64(m.PId)));
            });
            it.Children = childrenList.Where(m => m.PId == it.Id).ToList();
        }, whereLambda, pagination);

        var menuDtos = Mapper.Map<List<MenuDto>>(menus);
        menuDtos.ForEach(menu => { menu.Children = null; });
        return menuDtos;
    }


    public async Task<List<ExportRowModel>> DownloadAsync(MenuQueryCriteria menuQueryCriteria)
    {
        var menus = await QueryAsync(menuQueryCriteria, new Pagination { PageSize = 9999 });
        List<ExportRowModel> exportRowModels = new List<ExportRowModel>();
        List<ExportColumnModel> exportColumnModels;
        int point;
        menus.ForEach(menu =>
        {
            point = 0;
            exportColumnModels = new List<ExportColumnModel>
            {
                new() { Key = "菜单标题", Value = menu.Title, Point = point++ },
                new() { Key = "api路径", Value = menu.LinkUrl, Point = point++ },
                new() { Key = "路径", Value = menu.Path, Point = point++ },
                new() { Key = "权限代码", Value = menu.Permission, Point = point++ },
                new() { Key = "IFrame", Value = menu.IFrame ? "是" : "否", Point = point++ },
                new() { Key = "组件", Value = menu.Component, Point = point++ },
                new() { Key = "组件名称", Value = menu.ComponentName, Point = point++ },
                new() { Key = "父级ID", Value = menu.PId.IsNull() ? "null" : menu.PId.ToString(), Point = point++ },
                new() { Key = "排序", Value = menu.MenuSort.ToString(), Point = point++ },
                new() { Key = "Icon", Value = menu.Icon, Point = point++ },
                new() { Key = "类型", Value = GetMenuTypeName(menu.Type), Point = point++ },
                new() { Key = "缓存", Value = menu.Cache ? "是" : "否", Point = point++ },
                new() { Key = "显示", Value = menu.Hidden ? "否" : "是", Point = point++ },
                new() { Key = "子菜单数量", Value = menu.SubCount.ToString(), Point = point++ },
                new()
                {
                    Key = "创建时间", Value = menu.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"), Point = point++
                }
            };
            exportRowModels.Add(new ExportRowModel { exportColumnModels = exportColumnModels });
        });
        return exportRowModels;
    }

    #endregion

    #region 扩展方法

    public async Task<List<MenuDto>> QueryAllAsync()
    {
        var menuDtos = await _redisCacheService.GetCacheAsync<List<MenuDto>>("menus:LoadAllMenu");
        if (menuDtos.IsNotNull())
        {
            return menuDtos;
        }

        menuDtos = Mapper.Map<List<MenuDto>>(await BaseDal.QueryListAsync(m => m.IsDeleted == false));
        if (menuDtos.IsNotNull())
        {
            await _redisCacheService.SetCacheAsync("menus:LoadAllMenu", menuDtos, TimeSpan.FromSeconds(120));
        }

        return menuDtos;
    }

    /// <summary>
    /// 构建前端路由菜单
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns></returns>
    [RedisCaching(KeyPrefix = RedisKey.UserBuildMenuById)]
    public async Task<List<MenuTreeVo>> BuildTreeAsync(long userId)
    {
        var userRoles = await _userRolesService.QueryAsync(userId);
        var roleIds = new List<long>();
        roleIds.AddRange(userRoles.Select(r => r.RoleId));
        var menuList = await BaseDal.QueryMuchAsync<Menu, RoleMenu, MenuDto>(
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
                PId = m.PId,
                MenuSort = m.MenuSort,
                Icon = m.Icon,
                Type = m.Type,
                IsDeleted = m.IsDeleted,
                Id = m.Id,
                CreateTime = m.CreateTime,
                CreateBy = m.CreateBy
            },
            (m, rm) => m.IsDeleted == false && roleIds.Contains(rm.RoleId) && m.Type != (int)MenuType.Button
            , (m, rm) => new { m.Title, m.LinkUrl, m.Path, m.Permission, m.IFrame, m.Component, m.ComponentName, m.PId, m.MenuSort, m.Icon, m.Type, m.IsDeleted, m.Id, m.CreateBy, m.CreateTime },
            "menu_sort asc");
        var menuListChild = TreeHelper<MenuDto>.ListToTrees(menuList, "Id", "PId", null);
        return await BuildAsync(menuListChild);
    }


    [RedisCaching(Expiration = 30, KeyPrefix = RedisKey.LoadMenusById)]
    public async Task<List<MenuDto>> FindSuperiorAsync(long id)
    {
        Expression<Func<Menu, bool>> whereLambda = m => m.IsDeleted == false;
        var menu = await QuerySingleAsync(id);
        List<MenuDto> menuDtoList;
        if (menu.PId.IsNull())
        {
            var menus = await BaseDal.QueryListAsync(x => x.PId == null && x.IsDeleted == false, x => x.MenuSort,
                OrderByType.Asc);
            menuDtoList = Mapper.Map<List<MenuDto>>(menus);
            menuDtoList.ForEach(x => x.Children = null);
        }
        else
        {
            //查出同级菜单ID
            List<long> parentIds = new List<long>();
            parentIds.Add(Convert.ToInt64(menu.PId));
            await GetParentIdsAsync(menu, parentIds);
            whereLambda = whereLambda.And(m => parentIds.Contains(Convert.ToInt64(m.PId)) || m.PId == null);

            var menus = await BaseDal.QueryMapperAsync(async (it, cache) =>
            {
                if (parentIds.Contains(it.Id) && it.PId.IsNull())
                {
                    var childrenList = await cache.Get(list =>
                    {
                        //var ids = list.Select(i => i.ID).ToList();&& ids.Contains(m.PID)
                        return BaseDal.QueryListAsync(m => m.IsDeleted == false);
                    });
                    it.Children = childrenList.Where(m => m.PId == it.Id).ToList();
                }
            }, whereLambda, "menu_sort asc");


            var tempDtos = Mapper.Map<List<MenuDto>>(menus);
            menuDtoList = TreeHelper<MenuDto>.ListToTrees(tempDtos, "Id", "PId", null);
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

        menuDtOs.ForEach(async item =>
        {
            menuDtoList = item.Children;
            menuVo = new MenuTreeVo
            {
                Name = item.ComponentName.IsNullOrEmpty() ? item.Title : item.ComponentName,
                Path = item.PId.IsNull() ? "/" + item.Path : item.Path,
                Hidden = item.Hidden
            };

            if (!item.IFrame)
            {
                if (item.PId.IsNull())
                {
                    menuVo.Component = item.Component.IsNullOrEmpty() ? "Layout" : item.Component;
                }
                else if (!item.Component.IsNullOrEmpty())
                {
                    menuVo.Component = item.Component;
                }
            }

            menuVo.Meta = new MenuMetaVO(item.Title, item.Icon, !item.Cache);
            if (menuDtoList is { Count: > 0 })
            {
                menuVo.AlwaysShow = true;
                menuVo.Redirect = "noredirect";
                menuVo.Children = await BuildAsync(menuDtoList);
            }

            menuVos.Add(menuVo);
        });
        return await Task.FromResult(menuVos);
    }

    [RedisCaching(Expiration = 30, KeyPrefix = RedisKey.LoadMenusByPId)]
    public async Task<List<MenuDto>> FindByPIdAsync(long pid = 0)
    {
        List<MenuDto> menuDtos = null;
        Expression<Func<Menu, bool>> whereLambda = m => m.IsDeleted == false;
        whereLambda = pid == 0 ? whereLambda.And(m => m.PId == null) : whereLambda.And(m => m.PId == pid);

        menuDtos = Mapper.Map<List<MenuDto>>(await BaseDal.QueryListAsync(whereLambda, o => o.MenuSort,
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
        var menuList = await BaseDal.QueryMuchAsync<Menu, RoleMenu, MenuDto>(
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
                PId = m.PId,
                MenuSort = m.MenuSort,
                Icon = m.Icon,
                Type = m.Type,
                IsDeleted = m.IsDeleted,
                Id = m.Id,
                CreateTime = m.CreateTime,
                CreateBy = m.CreateBy
            },
            (m, rm) => m.IsDeleted == false && rm.IsDeleted == false && roleId == rm.RoleId
        );
        return menuList;
        // return TreeHelper<MenuDto>.SetLeafProperty(menuList, "Id", "PId", null);
    }

    private async Task<List<long>> GetParentIdsAsync(Menu m, List<long> parentIds)
    {
        var menu = await BaseDal.QueryFirstAsync(x => x.IsDeleted == false && x.Id == m.PId);
        if (menu.IsNull() || menu.PId.IsNull())
        {
            //parentIds.Add(menu.PId);
            return await Task.FromResult(parentIds);
        }

        parentIds.Add(Convert.ToInt64(menu.PId));
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
        menuList.ForEach(async ml =>
        {
            if (!ids.Contains(ml.Id))
            {
                ids.Add(ml.Id);
            }

            List<Menu> menus = await BaseDal.QueryListAsync(m => m.IsDeleted == false && m.PId == ml.Id);
            if (menus is { Count: > 0 })
            {
                await FindChildIdsAsync(menus, ids);
            }
        });

        await Task.FromResult(ids);
    }

    public async Task<List<long>> FindChildAsync(long id)
    {
        List<long> ids = new List<long> { id };
        var menus = await QueryAllAsync();
        if (menus.Count > 0)
        {
            var menus2 = menus.Where(x => x.PId == id).ToList();
            if (menus2.Count > 0)
            {
                var ids2 = menus2.Select(x => x.Id).ToList();
                ids.AddRange(ids2);

                var menus3 = menus.Where(x => ids2.Contains(Convert.ToInt64(x.PId))).ToList();
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
}