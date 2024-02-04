using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ape.Volo.Entity.Permission;
using Ape.Volo.IBusiness.Interface.Permission;
using SqlSugar;

namespace Ape.Volo.Business.Permission;

public class DataScopeService : IDataScopeService
{
    #region 字段

    private readonly ISqlSugarClient _db;

    #endregion

    #region 构造函数

    public DataScopeService(ISqlSugarClient db)
    {
        _db = db;
    }

    #endregion

    #region 基础方法

    /// <summary>
    /// 获取用户所有角色关联的部门ID
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="deptId"></param>
    /// <returns></returns>
    public async Task<List<long>> GetDataScopeDeptList(long userId, long deptId)
    {
        List<long> deptIds = new List<long>();
        //var user = await _userService.QueryByIdAsync(userId);
        var user = await _db.Queryable<User>().Where(x => x.Id == userId).Includes(x => x.Roles)
            .FirstAsync();
        if (user == null || user.Roles.Count == 0)
        {
            return deptIds;
        }

        //存在一个"全部"数据权限 直接返回空
        var isAll = user.Roles.Where(x => x.DataScope == "全部").Any();

        if (isAll)
        {
            return deptIds;
        }

        foreach (var role in user.Roles)
        {
            switch (role.DataScope)
            {
                case "本级":
                    deptIds.AddRange(await GetChildIds([deptId], null));
                    break;
                case "自定义":
                    var roleTmp = await _db.Queryable<Role>().Where(x => x.Id == role.Id)
                        .Includes(x => x.DepartmentList)
                        .FirstAsync();
                    if (roleTmp.DepartmentList.Count != 0)
                    {
                        List<long> ids = new List<long>();
                        ids.AddRange(roleTmp.DepartmentList.Select(x => x.Id));
                        deptIds.AddRange(await GetChildIds(ids, null));
                    }

                    break;
            }
        }

        if (!deptIds.Any())
        {
            deptIds.Add(0); //预防空数据
        }

        return deptIds.Distinct().ToList();
    }


    /// <summary>
    /// 获取所选部门及全部下级部门ID
    /// </summary>
    /// <param name="ids"></param>
    /// <param name="allIds"></param>
    /// <returns></returns>
    private async Task<List<long>> GetChildIds(List<long> ids, List<long> allIds)
    {
        allIds ??= new List<long>();

        foreach (var id in ids.Where(id => !allIds.Contains(id)))
        {
            allIds.Add(id);
            var list = await _db.Queryable<Department>().Where(x => x.ParentId == id && x.Enabled).ToListAsync();
            if (list.Any())
            {
                await GetChildIds(list.Select(x => x.Id).ToList(), allIds);
            }
        }

        return allIds;
    }

    #endregion
}
