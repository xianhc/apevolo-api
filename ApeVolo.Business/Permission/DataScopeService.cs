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

    public async Task<List<long>> GetDeptIds(UserDto userDto)
    {
        List<long> deptIds = new List<long>();
        // 查询用户角色
        List<RoleSmallDto> roleList = await _roleService.QueryByUserIdAsync(userDto.Id);

        //存在一个"全部"数据权限 直接返回空
        bool isAll = false;
        foreach (var item in roleList)
        {
            if (item.DataScope == "全部")
            {
                isAll = true;
                break;
            }
        }

        if (isAll)
        {
            return deptIds;
        }

        foreach (var role in roleList)
        {
            switch (role.DataScope)
            {
                case "本级":
                {
                    if (!deptIds.Contains(userDto.DeptId))
                    {
                        deptIds.Add(userDto.DeptId);
                    }

                    break;
                }
                case "自定义":
                    await GetCustomizeDeptIds(deptIds, role);
                    break;
            }
        }

        return deptIds;
    }

    private async Task GetCustomizeDeptIds(List<long> deptIds, RoleSmallDto role)
    {
        var roleDepts = await _roleDeptService.QueryByRoleIdAsync(role.Id);
        if (roleDepts.Any())
        {
            deptIds.AddRange(await _departmentService.GetChildIds(deptIds, null));
        }
    }

    #endregion
}
