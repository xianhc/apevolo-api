using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApeVolo.IBusiness.Dto.Permission;
using ApeVolo.IBusiness.Interface.Permission;

namespace ApeVolo.Business.Permission;

public class DataScopeService : IDataScopeService
{
    #region 字段

    private readonly IRoleService _roleService;
    private readonly IDepartmentService _departmentService;
    private readonly IRoleDeptService _roleDeptService;

    #endregion

    #region 构造函数

    public DataScopeService(IRoleService roleService, IDepartmentService departmentService,
        IRoleDeptService roleDeptService)
    {
        _roleService = roleService;
        _departmentService = departmentService;
        _roleDeptService = roleDeptService;
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
        List<RoleSmallDto> roleList = await _roleService.QueryByUserIdAsync(userId);

        //存在一个"全部"数据权限 直接返回空
        var isAll = roleList.Where(x => x.DataScope == "全部").Any();

        if (!isAll)
        {
            foreach (var role in roleList)
            {
                switch (role.DataScope)
                {
                    case "本级":
                        deptIds.AddRange(await _departmentService.GetChildIds(new List<long> { deptId }, null));
                        break;
                    case "自定义":
                        var roleDepts = await _roleDeptService.QueryByRoleIdAsync(role.Id);
                        if (roleDepts.Any())
                        {
                            List<long> ids = new List<long>();
                            ids.AddRange(roleDepts.Select(x => x.DeptId));
                            deptIds.AddRange(await _departmentService.GetChildIds(ids, null));
                        }

                        break;
                }
            }
        }

        if (!deptIds.Any())
        {
            deptIds.Add(0); //预防空数据
        }

        return deptIds.Distinct().ToList();
    }

    #endregion
}
