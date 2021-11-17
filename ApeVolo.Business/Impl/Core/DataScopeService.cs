using System.Collections.Generic;
using System.Threading.Tasks;
using ApeVolo.Common.DI;
using ApeVolo.IBusiness.Interface.Core;
using ApeVolo.IBusiness.Dto.Core;

namespace ApeVolo.Business.Impl.Core
{
    public class DataScopeService : IDataScopeService, IDependencyService
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

        public async Task<List<string>> GetDeptIds(UserDto userDto)
        {
            List<string> deptIds = new List<string>();
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

            roleList.ForEach(async role =>
            {
                if (role.DataScope == "本级")
                {
                    if (!deptIds.Contains(userDto.DeptId))
                    {
                        deptIds.Add(userDto.DeptId);
                    }
                }
                else if (role.DataScope == "自定义")
                {
                    await GetCustomizeDeptIds(deptIds, role);
                }
            });
            return deptIds;
        }

        private async Task GetCustomizeDeptIds(List<string> deptIds, RoleSmallDto role)
        {
            var roleDepts = await _roleDeptService.QueryByRoleIdAsync(role.Id);
            roleDepts.ForEach(async rd =>
            {
                if (!deptIds.Contains(rd.DeptId))
                {
                    deptIds.Add(rd.DeptId);
                }

                //如果部门存在子级部门 一并带出来
                List<DepartmentDto> departmentDtos = await _departmentService.QueryByPIdAsync(rd.DeptId);
                if (departmentDtos != null && departmentDtos.Count > 0)
                {
                    List<string> ids = await _departmentService.FindChildIds(deptIds, departmentDtos);
                    ids.ForEach(id =>
                    {
                        if (!deptIds.Contains(id))
                        {
                            deptIds.Add(id);
                        }
                    });
                }
            });
        }

        #endregion
    }
}